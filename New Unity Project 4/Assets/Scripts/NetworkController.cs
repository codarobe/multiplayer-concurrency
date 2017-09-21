using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkController : MonoBehaviour {
	public int maxConnections;
	private int hostID;
	private int connectionID;
	private int myReliableChannelID;
	private int myUnreliableChannelID;

	private int[] connections;
	private int currentConnections = 0;

	public bool isHost;
	public string ip;
	public int port;
	// Use this for initialization
	void Start () {
		// Initializing the Transport Layer with no arguments (default settings)
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
		//receiveData ();
		//Debug.Log (error);

	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < connections.Length; i++) {
            sendData();
			receiveData();
		}
	}

	void sendData() {
		byte error;

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
        MovementActionMessage message = new MovementActionMessage(moveHorizontal, moveVertical, 0);

        byte[] messageArray = message.toByteArray();

        NetworkTransport.Send(hostID, connectionID, myReliableChannelID, messageArray, messageArray.Length,  out error);
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
                Debug.Log(hostID);
                Debug.Log(connectionID);
                Debug.Log(recHostID);
                Debug.Log(recConnectionID);
			//myConnectionID
			if ((recHostID == connectionID) && (currentConnections < maxConnections)) {
				//my active connect request approved
				Debug.Log ("Successfully Connected");
				connections [currentConnections] = connectionID;
				currentConnections++;
			} else {
				//somebody else connect to me
				Debug.Log ("Connection Rejected");
			}
			Debug.Log (currentConnections);
			break;
		case NetworkEventType.DataEvent:       //3
                Debug.Log("Data");
                MovementActionMessage message = new MovementActionMessage(recBuffer);
                Debug.Log(message.getX());
                Debug.Log(message.getY());
                Debug.Log(message.getAction());

                GameObject opponent = GameObject.Find("Opponent");
                Debug.Log(opponent);
                Done_PlayerController script = opponent.GetComponent<Done_PlayerController>();
                script.Move(message.getX(), message.getY());
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
			currentConnections--;
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
