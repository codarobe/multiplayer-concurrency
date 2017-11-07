using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkManager
{

	private bool ready = false;
	
	private string id = "";
	private bool idSet = false;
	
	public GameObject player;
	
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

	public void receiveMessage()
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
	}

	// possibly remove this method.  create specific methods to handle each relevant message type
	public void handleMessage(byte[] message, NetworkEventType eventType)
	{

		string recID;

		int messageType;

		switch (eventType)
		{
			case NetworkEventType.Nothing:         
				break;
			case NetworkEventType.DataEvent:       
				Debug.Log("Data");
				MemoryStream ms = new MemoryStream();
				MovementActionMessage movementActionMessage = new MovementActionMessage(message);
                synchronize(movementActionMessage);
				
				break;
			case NetworkEventType.DisconnectEvent: //4
				Debug.Log(id + " disconnected");
				NetworkConfiguration.networkController.disconnect(hostID, connectionID);
				
				break;
		}
		
		// If we haven't set the player to the correct ID yet, do it
		if (idSet == false)
		{
			//id = recID;
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

	private void setIdentifier()
	{
		PlayerIDController idController = player.GetComponent<PlayerIDController>();
		idController.text = id;
		idController.messagePermanent = true;
	}

	public string getIdentifier()
	{
		return id;
	}

	public void setSpawnPoint(int spawnPoint)
	{
		spawnID = spawnPoint;
	}

	public void spawnPlayer()
	{
		// get spawnpoint
		GameObject spawnPoint = GameObject.Find("SpawnPoints/spawn" + spawnID);
		Transform transform = spawnPoint.GetComponent<Transform>().transform;
		Vector3 rotation = spawnPoint.GetComponent<Quaternion>().eulerAngles;

		// create game object
		

		// Set ID
		//PlayerIDController idController = opponent.GetComponent<PlayerIDController>();
		//idController.text = message.getID();
		//idController.messagePermanent = true;
		//idSet = true;


		// set location/rotation to designated spawn point
	}
}
