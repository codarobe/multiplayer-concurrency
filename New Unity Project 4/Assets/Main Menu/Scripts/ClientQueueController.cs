using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ClientQueueController : MonoBehaviour
{
	public Text connectedPlayers;

	private bool readyToStart = false;
	
	private void OnEnable()
	{
		Debug.Log("Queue enabled!");

		// Initialize fields
		string playerID = GameObject.Find("Multiplayer/SettingsWindow/NameInput").GetComponent<InputField>().text;
		if (playerID != "") {
			NetworkConfiguration.playerName = playerID;
		}
		string ip = GameObject.Find("Multiplayer/SettingsWindow/IPInput").GetComponent<InputField>().text;
		if (ip != "") {
			NetworkConfiguration.ipAddress = ip;
		}
		string portInput = GameObject.Find("Multiplayer/SettingsWindow/RemotePortNum").GetComponent<InputField>().text;
		int port;
		if (Int32.TryParse(portInput, out port)) {
			NetworkConfiguration.remotePort = port;
		}
		portInput = GameObject.Find("Multiplayer/SettingsWindow/LocalPortNum").GetComponent<InputField>().text;
		if (Int32.TryParse(portInput, out port)) {
			NetworkConfiguration.localPort = port;
		}

		NetworkConfiguration.isHost = false;
		
		Debug.Log("IP: " + NetworkConfiguration.ipAddress);
		Debug.Log("Local Port: " + NetworkConfiguration.localPort);
		Debug.Log("Remote Port: " + NetworkConfiguration.remotePort);

		connectedPlayers.text = "(Local) " + NetworkConfiguration.playerName + ": Pending...";
		
		NetworkConfiguration.allowConnections = true;
		NetworkConfiguration.networkController = new NetworkController();
	}
	
	public void updateConnectedPlayers(string players)
	{	
		connectedPlayers.text = players;
	}

	private void OnDisable()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
		NetworkConfiguration.networkController.receiveData();
		
		if (NetworkConfiguration.networkController.ConnectedPlayersMessage != "")
		{
			updateConnectedPlayers(NetworkConfiguration.networkController.ConnectedPlayersMessage);
		}
		
	}
}
