using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientNetworkManager : MonoBehaviour
{

	private bool isConnected = false;
	
	private String id;
	
	public GameObject player;
	
	private int hostID;
	private int connectionID;
	private int unreliableChannelID;
	private int stateUpdateChannelID;

	public ClientNetworkManager(String identifier, int host, int connection, int unreliableID, int stateUpdateID)
	{
		id = identifier;
		hostID = host;
		connectionID = connection;
		unreliableChannelID = unreliableID;
		stateUpdateChannelID = stateUpdateID;
	}

	private void Update()
	{
		if (isConnected)
		{
			
		}
	}

	private bool sendMessage()
	{

		
		return false;
	}

	private void receiveMessage()
	{
		
	}

	private void handleMessage()
	{
		
	}
}
