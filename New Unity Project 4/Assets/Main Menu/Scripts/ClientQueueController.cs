using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ClientQueueController : MonoBehaviour
{
	public Text connectedPlayers;
	public Button startButton;

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
			NetworkConfiguration.localPort = port;
		}
		portInput = GameObject.Find("Multiplayer/SettingsWindow/LocalPortNum").GetComponent<InputField>().text;
		if (Int32.TryParse(portInput, out port)) {
			NetworkConfiguration.remotePort = port;
		}

		NetworkConfiguration.isHost = false;

		connectedPlayers.text = "(Local) " + NetworkConfiguration.playerName + ": Ready!";
		
		NetworkConfiguration.networkController = new NetworkController();
		NetworkConfiguration.allowConnections = true;
	}

	private void OnDisable()
	{
		Debug.Log("Queue disabled!");
		NetworkConfiguration.allowConnections = false;
		NetworkConfiguration.networkController.disconnectAll();
		NetworkConfiguration.networkController = null;
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}
