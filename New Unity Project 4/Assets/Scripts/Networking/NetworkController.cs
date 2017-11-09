using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


public class NetworkController
{
	public int maxConnections = 8;
	private int localHostID;
	private int unreliableChannelID;
	private int stateUpdateChannelID;

	private int hostConnectionID = -1;
	private int localSpawn;

	private int currentConnections = 0;
	public string originalPlayerMessage = "";
	public string ConnectedPlayersMessage = "";

	public bool localPlayerReady = false;

	private Dictionary<int, ClientNetworkManager> clientConnections;

	public string ip = "127.0.0.1";
	public int localPort = 4000;
    public int remotePort = 4000;

	public GameObject opponentPrefab;

	// Use this for initialization
	public NetworkController()
	{
		// Initializing the Transport Layer with no arguments (default settings)
		NetworkTransport.Init();

		clientConnections = new Dictionary<int, ClientNetworkManager>(maxConnections);

        if (NetworkConfiguration.ipAddress != "") {
            ip = NetworkConfiguration.ipAddress;
        }
        if (NetworkConfiguration.localPort != -1) {
            localPort = NetworkConfiguration.localPort;
        }
        if (NetworkConfiguration.remotePort != -1) {
            remotePort = NetworkConfiguration.remotePort;
        }

		ConnectionConfig config = new ConnectionConfig();
		// UDP Connection
		unreliableChannelID = config.AddChannel(QosType.UnreliableSequenced);
		// TCP Connection
		stateUpdateChannelID = config.AddChannel(QosType.StateUpdate);


		HostTopology topology = new HostTopology(config, maxConnections);

		// Initialized as host
		localHostID = NetworkTransport.AddHost(topology, localPort);


		if (!NetworkConfiguration.isHost)
		{
			// If not host, connect to host
			connect(ip, remotePort);

		}
		else
		{
			localPlayerReady = true;
		}

	}

	public void sendMessage(int peer, byte[] message)
	{
		ClientNetworkManager manager = null;
		clientConnections.TryGetValue(peer, out manager);
		if (manager != null)
		{
			manager.sendMessage(message);
		}
	}


	public void broadcastMovementAction()
	{
		byte error;

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		int action = (Input.GetButton("Fire1")) ? 1 : 0;
		action = (Input.GetButton("Fire2")) ? 2 : action;
		MovementActionMessage message = new MovementActionMessage(moveHorizontal, moveVertical, action, NetworkConfiguration.playerName);

		byte[] messageArray = message.toByteArray();
		
		broadcastMessage(messageArray);
	}

	public void broadcastMessage(byte[] messageArray)
	{
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			kvp.Value.sendMessage(messageArray);
			
		}
	}
	
	public void receiveData()
	{
		//Debug.Log(currentConnections);
		
		int remoteHostID;
		int remoteConnectionID;
		int channelID;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;

		// May throw a MessageToLong error

		NetworkEventType recData = NetworkTransport.Receive(out remoteHostID, out remoteConnectionID, out channelID, recBuffer, bufferSize, out dataSize, out error);

		//Debug.Log (error);

		if (dataSize > 0)
		{
			Debug.Log("Bytes received: " + dataSize);
		}
		
		
		

		switch (recData)
		{
			case NetworkEventType.Nothing:         //1
				break;
			case NetworkEventType.ConnectEvent:    //2
				Debug.Log("<color=green>Connection Found</color>");
                Debug.Log("Host ID: " + localHostID);
                Debug.Log("RecHost ID: " + remoteHostID);
                Debug.Log("RecConnection ID: " + remoteConnectionID);
				//myConnectionID
				if (NetworkConfiguration.allowConnections && (remoteHostID == localHostID) && (currentConnections < maxConnections))
				{
					//my active connect request approved
					Debug.Log("<color=green>Successfully Connected</color>");
					
					// add connection to connection list
					ClientNetworkManager manager =
						new ClientNetworkManager(remoteHostID, remoteConnectionID, unreliableChannelID, stateUpdateChannelID);
					clientConnections.Add(remoteConnectionID, manager);
					StateUpdateMessage stateUpdateMessage = new StateUpdateMessage(3);
					manager.sendMessage(stateUpdateMessage.toByteArray());
					Debug.Log("Successfully connected.  Sent registration message.");
					
					// broadcast connected users' information
					if (NetworkConfiguration.isHost)
					{
						sendRegisteredUsers(remoteConnectionID);
						Debug.Log("Sent Registered Users");
					}
					else
					{
						if (hostConnectionID == -1)
						{
							hostConnectionID = remoteConnectionID;
						}
					}
					
					currentConnections++;
				}
				else
				{
					//somebody else connect to me
					Debug.LogWarning("<color=red>Connection Rejected</color>");
				}
				Debug.Log(currentConnections + " Players Connected!");
				break;
			case NetworkEventType.DataEvent:       //3
				// route the message to the correct controller
				Debug.Log("Data");
				ClientNetworkManager clientNetworkManager = null;
				clientConnections.TryGetValue(remoteConnectionID, out clientNetworkManager);
				if (clientNetworkManager != null)
				{
					clientNetworkManager.handleMessage(recBuffer, dataSize, recData);
				}
				break;
			case NetworkEventType.DisconnectEvent: //4
				Debug.Log("Disconnection");
				//myConnectionID
				if (remoteHostID == localHostID)
				{
					//cannot connect by some reason see error
					Debug.LogWarning("Error: Lost connection.");
					disconnect(remoteHostID, remoteConnectionID);
				}
				else
				{
					//one of the established connection has been disconnected
					Debug.Log("Disconnected");
				}
				currentConnections--;
				break;
		}

		updateConnectedPlayerMessage();


	}

	public void updateConnectedPlayerMessage()
	{
		string message = "";
		if (NetworkConfiguration.isHost)
		{
			message += "(HOST)" + " " + NetworkConfiguration.playerName + ": ";

		}
		else
		{
			message += "(LOCAL)" + " " + NetworkConfiguration.playerName + ": ";
		}
		if (localPlayerReady)
		{
			message += "Ready!\n ";
		}
		else
		{
			message += "Pending...\n ";
		}

		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			if (!NetworkConfiguration.isHost && kvp.Key == hostConnectionID)
			{
				message += "(HOST) ";
			}
			message += kvp.Value.getIdentifier() + ": ";
			if (kvp.Value.isReady())
			{
				message += "Ready!\n ";
			}
			else
			{
				message += "Pending...\n ";
			}
		}

		ConnectedPlayersMessage = message;
	}

	public void sendRegisteredUsers(int client)
	{
		string[] identifiers = new string[currentConnections];
		string[] ipAddresses = new string[currentConnections];
		int[] ports = new int[currentConnections];
		bool[] statuses = new bool[currentConnections];

		int i = 0;
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			if (kvp.Key != client)
			{
				NetworkID network;
				NodeID dstNode;
				byte error;
				string ipAddress;
				int port;
				NetworkTransport.GetConnectionInfo(localHostID, kvp.Key, out ipAddress, out port, out network, out dstNode,
					out error);
				identifiers[i] = kvp.Value.getIdentifier();
				ipAddresses[i] = ipAddress;
				ports[i] = port;
				statuses[i] = kvp.Value.isReady();

				i++;
			}
			
		}
		UserListMessage userListMessage = new UserListMessage(identifiers, ipAddresses, ports, statuses);
		
		sendMessage(client, userListMessage.toByteArray());
	}

	public bool arePlayersReady()
	{
		if (currentConnections < 1)
		{
			return false;
		}

		bool status = true;
		
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			if (!kvp.Value.isReady())
			{
				status = false;
			}
		}
		return status;
	}

	public void disconnect(int hostID, int connectionID)
	{
		byte error;
		clientConnections.Remove(hostID);
		currentConnections--;
		NetworkTransport.Disconnect(hostID, connectionID, out error);
	}

	public void connect(string ipAddress, int port)
	{
		byte error;
		int connectionID = NetworkTransport.Connect(localHostID, ipAddress, port, 0, out error);
		Debug.Log("Sent Connection to Host");
		NetworkError networkError = (NetworkError) error;
		if (networkError == NetworkError.Ok)
		{
			//ClientNetworkManager manager = new ClientNetworkManager(localHostID, connectionID, unreliableChannelID, stateUpdateChannelID);
			//clientConnections.Add(connectionID, manager);
			//currentConnections++;
			//StateUpdateMessage stateUpdateMessage = new StateUpdateMessage(3);
			//manager.sendMessage(stateUpdateMessage.toByteArray());
			//Debug.Log("Successfully connected.  Sent registration message.");
		}
		else
		{
			Debug.Log("Error connecting to host");
		}
	}

	public void connectWithID(string ipAddress, int port, string identifier)
	{
		byte error;
		int connectionID = NetworkTransport.Connect(localHostID, ipAddress, port, 0, out error);
		NetworkError networkError = (NetworkError) error;
		if (networkError == NetworkError.Ok)
		{
			ClientNetworkManager manager = new ClientNetworkManager(identifier, localHostID, connectionID, unreliableChannelID,
				stateUpdateChannelID);
			clientConnections.Add(connectionID, manager);
			currentConnections++;
			StateUpdateMessage stateUpdateMessage = new StateUpdateMessage(3);
			manager.sendMessage(stateUpdateMessage.toByteArray());
			
		}
	}

	public void connectAll(string[] ipAddresses, int[] ports, string[] identifiers)
	{
		for (int i = 0; i < ipAddresses.Length; i++)
		{
			connectWithID(ipAddresses[i], ports[i], identifiers[i]);
		}
	}

	public void disconnectAll()
	{
		NetworkConfiguration.allowConnections = false;
		// iterate through list and disconnect everyone
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			byte error;
			NetworkTransport.Disconnect(localHostID, kvp.Key, out error);
			currentConnections--;
		}

		clientConnections = null;
		
		
		NetworkTransport.Shutdown();
	}

	public void startGame()
	{
		int i = 1;

		string[] ids = new string[currentConnections];
		int[] spawns = new int[currentConnections];
		
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			kvp.Value.setSpawnPoint(i);
			ids[i] = kvp.Value.getIdentifier();
			spawns[i] = i;
			i++;
		}

		localSpawn = i;

		// create message and broadcast
	}

	public void spawnPlayers()
	{
		// spawn remote players
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			kvp.Value.spawnPlayer(false);
		}
		// spawn local player
	}
}
