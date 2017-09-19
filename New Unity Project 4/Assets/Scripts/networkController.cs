using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkController : MonoBehaviour {
	public int maxConnections;
	private int hostID;
	private int connectionID;
	private int myReliableChannelID;
	private int myUnreliableChannelID;

	public bool isHost;
	public string ip;
	public int port;

	// Use this for initialization
	void Start () {
		// Initializing the Transport Layer with no arguments (default settings)
		NetworkTransport.Init();

		ConnectionConfig config = new ConnectionConfig();
		// TCP Connection
		myReliableChannelID  = config.AddChannel(QosType.Reliable);
		// UDP Connection
		myUnreliableChannelID = config.AddChannel(QosType.Unreliable);

		if (isHost) {
			HostTopology topology = new HostTopology (config, maxConnections);

			// Initialized as host
			hostID = NetworkTransport.AddHost (topology, 8888);
		} else {
			connectionID = NetworkTransport.Connect(hostID, "192.16.7.21", 8888, 0, out error);

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isHost) {
			receiveData ();
		} else {
			sendData ();
		}
	}

	void sendData() {
		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;

		NetworkTransport.Send(hostID, connectionID, myReiliableChannelID, buffer, bufferLength,  out error);
	}

	void receiveData() {
		int recHostId; 
		int connectionId; 
		int channelId; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;

		// May throw a MessageToLong error
		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
		switch (recData)
		{
		case NetworkEventType.Nothing:         //1
			//Debug.Log("nothing");
			break;
		case NetworkEventType.ConnectEvent:    //2
			Debug.Log("Connection Found");
			//myConnectionID
			if(hostID == connectionId)
				//my active connect request approved
				Debug.Log("Successfully Connected");
			else
				//somebody else connect to me
				Debug.Log("Connection Rejected");
			break;
		case NetworkEventType.DataEvent:       //3
			Debug.Log("Data");
			break;
		case NetworkEventType.DisconnectEvent: //4
			Debug.Log("Disconnection");
			//myConnectionID
			if(hostID == connectionId)
				//cannot connect by some reason see error
				Debug.Log("Error: Lost connection.");
			else
				//one of the established connection has been disconnected
				Debug.Log("Disconnected");
			break;
		}
	}

	void onApplicationQuit() {
		byte error;

		NetworkTransport.Disconnect(hostID, connectionID, out error);
	}
}
