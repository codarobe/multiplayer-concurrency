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

	private int[] connections;

	public bool isHost;
	public string ip;
	public int port;

	// Use this for initialization
	void Start () {
		// Initializing the Transport Layer with no arguments (default settings)
		Debug.Log("HERE");
		NetworkTransport.Init();

		connections = new int[maxConnections];

		ConnectionConfig config = new ConnectionConfig();
		// TCP Connection
		myReliableChannelID  = config.AddChannel(QosType.Reliable);
		// UDP Connection
		myUnreliableChannelID = config.AddChannel(QosType.Unreliable);


		HostTopology topology = new HostTopology (config, maxConnections);

		// Initialized as host
		hostID = NetworkTransport.AddHost (topology, port);

		byte error;
		connectionID = NetworkTransport.Connect(hostID, ip, port, 0, out error);
		receiveData ();
		//Debug.Log (error);
	}
	
	// Update is called once per frame
	void Update () {
		sendData ();
		for (int i = 0; i < connections.Length; i++) {
			receiveData ();
		}
	}

	void sendData() {
		int recHostID; 
		int recConnectionID; 
		int channelID; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;

		NetworkTransport.Send(hostID, connectionID, myReliableChannelID, recBuffer, bufferSize,  out error);
	}

	void receiveData() {
		int recHostID; 
		int recConnectionID; 
		int channelID; 
		byte[] recBuffer = new byte[1024]; 
		int bufferSize = 1024;
		int dataSize;
		byte error;

		// May throw a MessageToLong error

		NetworkEventType recData = NetworkTransport.Receive(out recHostID, out recConnectionID, out channelID, recBuffer, bufferSize, out dataSize, out error);

		//Debug.Log (error);

		switch (recData)
		{
		case NetworkEventType.Nothing:         //1
			//Debug.Log("nothing");
			break;
		case NetworkEventType.ConnectEvent:    //2
			Debug.Log ("Connection Found");
			//myConnectionID
			if ((hostID == connectionID) && (connections.Length < maxConnections)) {
				//my active connect request approved
				Debug.Log ("Successfully Connected");
				connections[connections.Length] = connectionID;
			} else {
				//somebody else connect to me
				Debug.Log ("Connection Rejected");
			}
			break;
		case NetworkEventType.DataEvent:       //3
			Debug.Log("Data");
			break;
		case NetworkEventType.DisconnectEvent: //4
			Debug.Log ("Disconnection");
			//myConnectionID
			if (hostID == connectionID) {
				//cannot connect by some reason see error
				Debug.Log ("Error: Lost connection.");
			} else {
				//one of the established connection has been disconnected
				Debug.Log ("Disconnected");
			}
			break;
		}
		
	
	}

	void onApplicationQuit() {
		byte error;

		NetworkTransport.Disconnect(hostID, connectionID, out error);

		Debug.Log ("disconnected");
		//Debug.Log (error);
	}
}
