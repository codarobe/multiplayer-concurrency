using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkManager : MonoBehaviour
{

	private bool isConnected = false;
	
	private string id;
	
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

	public void handleMessage(byte[] message)
	{
		
	}

	public void setIdentifier(string identifier)
	{
		id = identifier;
	}

	public string getIdentifier()
	{
		return id;
	}

	public void setSpawnPoint(int spawnPoint)
	{
		spawnID = spawnPoint;
	}
}
