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

	private int currentConnections = 0;

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
		// TCP Connection
		unreliableChannelID = config.AddChannel(QosType.UnreliableSequenced);
		// UDP Connection
		stateUpdateChannelID = config.AddChannel(QosType.StateUpdate);


		HostTopology topology = new HostTopology(config, maxConnections);

		// Initialized as host
		localHostID = NetworkTransport.AddHost(topology, localPort);

	
		if (!NetworkConfiguration.isHost)
		{
			byte error;
			int connectionID = NetworkTransport.Connect(localHostID, ip, remotePort, 0, out error);
			NetworkError networkError = (NetworkError) error;
			if (networkError == NetworkError.Ok)
			{
				//clientConnections.Add(connectionID, new ClientNetworkManager(localHostID, connectionID, unreliableChannelID, stateUpdateChannelID));
			}
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
					clientConnections.Add(remoteConnectionID, new ClientNetworkManager(localHostID, remoteConnectionID, unreliableChannelID, stateUpdateChannelID));
					
					// broadcast connected users' information
					sendRegisteredUsers(remoteConnectionID);
					
					currentConnections++;
				}
				else
				{
					//somebody else connect to me
					Debug.LogWarning("Connection Rejected");
				}
				Debug.Log(currentConnections);
				break;
			case NetworkEventType.DataEvent:       //3
				Debug.Log("Data");
				MemoryStream ms = new MemoryStream();
				MovementActionMessage message = new MovementActionMessage(recBuffer);
                Debug.Log("X: " + message.getX());
                Debug.Log("Y: " + message.getY());
                Debug.Log("Action: " + message.getAction());
                Debug.Log("ID: " + message.getID());


				GameObject opponent = GameObject.Find("Opponent");
				Debug.Log(opponent);
                
				Done_PlayerController script = opponent.GetComponent<Done_PlayerController>();
				script.Move(message.getX(), message.getY());
				script.executeAction(message.getAction());
				break;
			case NetworkEventType.DisconnectEvent: //4
				Debug.Log("Disconnection");
				//myConnectionID
				if (remoteHostID == localHostID)
				{
					//cannot connect by some reason see error
					Debug.LogWarning("Error: Lost connection.");
				}
				else
				{
					//one of the established connection has been disconnected
					Debug.Log("Disconnected");
				}
				currentConnections--;
				break;
		}


	}

	public void sendRegisteredUsers(int client)
	{
		string[] identifiers = new String[currentConnections];
		string[] ipAddresses = new string[currentConnections];
		int[] ports = new int[currentConnections];

		int i = 0;
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
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

			i++;
		}
		UserListMessage userListMessage = new UserListMessage(identifiers, ipAddresses, ports);
		broadcastMessage(userListMessage.toByteArray());
	}

	public void disconnect(int hostID, int connectionID)
	{
		byte error;
		clientConnections.Remove(hostID);
		NetworkTransport.Disconnect(hostID, connectionID, out error);
	}

	public void disconnectAll()
	{
		// iterate through list and disconnect everyone
		foreach (KeyValuePair<int, ClientNetworkManager> kvp in clientConnections)
		{
			byte error;
			NetworkTransport.Disconnect(localHostID, kvp.Key, out error);
		}
		
		NetworkTransport.Shutdown();
	}
}
