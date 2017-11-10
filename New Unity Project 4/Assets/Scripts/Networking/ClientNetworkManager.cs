using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ClientNetworkManager
{

	private bool ready = false;
	
	private string id = "";
	private bool idSet = false;
	
	public GameObject player = null;
	
	private int hostID;
	private int connectionID;
	private int unreliableChannelID;
	private int stateUpdateChannelID;

	private int spawnID;

	public ClientNetworkManager(int host, int connection, int unreliableID, int stateUpdateID)
	{
		// Set dummy id
		id = "Player " + hostID;
		
		// Set connection information
		hostID = host;
		connectionID = connection;
		unreliableChannelID = unreliableID;
		stateUpdateChannelID = stateUpdateID;
	}

	public ClientNetworkManager(string identifier, int host, int connection, int unreliableID, int stateUpdateID)
	{
		// Set identifier
		id = identifier;
		idSet = true;
		
		// Set connection information
		hostID = host;
		connectionID = connection;
		unreliableChannelID = unreliableID;
		stateUpdateChannelID = stateUpdateID;
	}

	public void sendMessage(byte[] message)
	{
		// Send message
		byte error;
		NetworkTransport.Send(hostID, connectionID, stateUpdateChannelID, message, message.Length, out error);
	}

	/*public void receiveMessage()
	{
		// Receive message from connected host
		int remoteConnectionID;
		int remoteChannelID;
		int bufferSize = 1024;
		byte[] recBuffer = new byte[bufferSize];
		int dataSize;
		byte error;
		NetworkEventType eventType = NetworkTransport.ReceiveFromHost(hostID, out remoteConnectionID, out remoteChannelID, recBuffer, bufferSize, out dataSize, out error);
		
		handleMessage(recBuffer, eventType);
	}*/

	// possibly remove this method.  create specific methods to handle each relevant message type
	public void handleMessage(byte[] message, int messageSize, NetworkEventType eventType)
	{

		string recID;

		switch (eventType)
		{
			case NetworkEventType.Nothing:         
				break;
			case NetworkEventType.DataEvent:   
				Debug.Log("Client data event");
				MemoryStream ms = new MemoryStream(message);
				byte[] buffer = new byte[4];
				ms.Read(buffer, 0, 4);
				int messageType = System.BitConverter.ToInt32(buffer, 0);
				buffer = new byte[2048];
				int size = messageSize - 4;
				ms.Read(buffer, 0,  size);

				switch (messageType)
				{
					case MessageType.MOVEMENT_ACTION:
						Debug.Log("Movement/Action Message");
						MovementActionMessage movementActionMessage = new MovementActionMessage(buffer);
						synchronize(movementActionMessage);
						break;
					case MessageType.STATE_UPDATE:
						Debug.Log("State Update Message");
						StateUpdateMessage stateUpdateMessage = new StateUpdateMessage(buffer);
						// registration event
						if (stateUpdateMessage.getUpdateType() == 3)
						{
							Debug.Log("Registered player with id: " + stateUpdateMessage.getID());
							if (!idSet)
							{
								id = stateUpdateMessage.getID();
								idSet = true;
							}
							if (NetworkConfiguration.isHost)
							{
								NetworkConfiguration.networkController.sendRegisteredUsers(connectionID);
							}
						}
						// ready event
						else if (stateUpdateMessage.getUpdateType() == 2)
						{
							Debug.Log("Ready Event");
							ready = true;
							id = stateUpdateMessage.getID();
						}
						break;
					case MessageType.USER_LIST:
						Debug.Log("User List Message");
						UserListMessage listMessage = new UserListMessage(buffer);
						NetworkConfiguration.networkController.connectAll(listMessage.getIPs(), listMessage.getPorts(), listMessage.getIDs());

						if (!idSet)
						{
							id = listMessage.getHostID();
							idSet = true;
						}
						
						StateUpdateMessage readyMessage = new StateUpdateMessage(2);
						NetworkConfiguration.networkController.localPlayerReady = true;
						ready = true;
						NetworkConfiguration.networkController.broadcastMessage(readyMessage.toByteArray());
						break;
					case MessageType.START_GAME:
						Debug.Log("START GAME");
						NetworkConfiguration.allowConnections = false;
						StartGameMessage gameMessage = new StartGameMessage(buffer);
						NetworkConfiguration.networkController.setSpawns(gameMessage.getIDs(), gameMessage.getSpawns(), gameMessage.getHostSpawn());
						SceneManager.LoadScene(1);
						break;
					default:
						Debug.Log("Error: wrong data event type: " + messageType);
						break;
				}
				
				break;
			case NetworkEventType.DisconnectEvent: //4
				Debug.Log(id + " disconnected");
				NetworkConfiguration.networkController.disconnect(hostID, connectionID);
				
				break;
		}
	}

	// update player position/orientation/action
	public void synchronize(MovementActionMessage message)
	{
		// Move player and execute pending action
		Done_PlayerController script = player.GetComponent<Done_PlayerController>();
		script.Move(message.getX(), message.getY());
		script.executeAction(message.getAction());
	}

	/*
	private void setIdentifier()
	{
		PlayerIDController idController = player.GetComponent<PlayerIDController>();
		idController.text = id;
		idController.messagePermanent = true;
	}
	*/

	public string getIdentifier()
	{
		return id;
	}

	public void setSpawnPoint(int spawnPoint)
	{
		spawnID = spawnPoint;
	}

	public int getSpawnPoint()
	{
		return spawnID;
	}

	/*
	public void spawnPlayer(bool local)
	{
		// get spawnpoint
		//GameObject spawnPoint = GameObject.Find("SpawnPoints/spawn" + spawnID);
		//Vector3 transform = spawnPoint.GetComponent<Transform>().position;
		//Quaternion rotation = spawnPoint.GetComponent<Transform>().rotation;

		// create game object
		if (local)
		{
			//player = Instantiate(GameObject.Find("SpawnPoints").GetComponent<OpponentPrefab>().player, transform, rotation);
		}
		else
		{
			//player = Instantiate(GameObject.Find("SpawnPoints").GetComponent<OpponentPrefab>().opponent, transform, rotation);
		}

		// Set ID
		//PlayerIDController idController = player.GetComponent<PlayerIDController>();
		//idController.text = id;
		//idController.messagePermanent = true;

	}
	*/

	public bool isReady()
	{
		return ready;
	}
}
