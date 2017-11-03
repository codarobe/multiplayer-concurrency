using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkManager
{

	private bool isConnected = false;
	
	private string id = null;
	
	public GameObject player;
	
	private int hostID;
	private int connectionID;
	private int unreliableChannelID;
	private int stateUpdateChannelID;

	private int spawnID;

	public ClientNetworkManager(int host, int connection, int unreliableID, int stateUpdateID)
	{
		hostID = host;
		connectionID = connection;
		unreliableChannelID = unreliableID;
		stateUpdateChannelID = stateUpdateID;
	}

	public bool sendMessage(byte[] message)
	{
		byte error;
		NetworkTransport.Send(hostID, connectionID, stateUpdateChannelID, message, message.Length, out error);
		
		return false;
	}

	// possibly remove this method.  create specific methods to handle each relevant message type
	public void handleMessage(byte[] message)
	{
		
	}

	public void synchonize(MovementActionMessage message)
	{
		// If id hasn't been set, set ID
		if (id == null)
		{
			setIdentifier(message.getID());
		}
		// Move player and execute pending action
		Done_PlayerController script = player.GetComponent<Done_PlayerController>();
		script.Move(message.getX(), message.getY());
		script.executeAction(message.getAction());
	}

	public void setIdentifier(string identifier)
	{
		id = identifier;
		PlayerIDController idController = player.GetComponent<PlayerIDController>();
		idController.text = identifier;
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
		// create game object
		
		// set location/rotation to designated spawn point
	}
}
