using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkController : MonoBehaviour
{
	public int maxConnections;
	private int hostID;
	private int connectionID;
	private int myReliableChannelID;
	private int myUnreliableChannelID;

	private int[] connections;
	private int currentConnections = 0;
    private bool isConnected;

	public bool isHost;
	public string ip;
	public int localPort;
    public int remotePort;

    private bool idSet = false;
	// Use this for initialization
	void Start()
	{
		// Initializing the Transport Layer with no arguments (default settings)
		NetworkTransport.Init();

        isConnected = false;

		connections = new int[maxConnections];

        if (NetworkConfiguration.ipAddress != "") {
            ip = NetworkConfiguration.ipAddress;
        }
        if (NetworkConfiguration.localPort != -1) {
            localPort = NetworkConfiguration.localPort;
        }
        if (NetworkConfiguration.remotePort != -1) {
            remotePort = NetworkConfiguration.remotePort;
        }

		ConnectionConfig config = new ConnectionConfig();
		// TCP Connection
		myReliableChannelID = config.AddChannel(QosType.Reliable);
		// UDP Connection
		myUnreliableChannelID = config.AddChannel(QosType.Unreliable);


		HostTopology topology = new HostTopology(config, maxConnections);

		// Initialized as host
		hostID = NetworkTransport.AddHost(topology, localPort);

		byte error;
		connectionID = NetworkTransport.Connect(hostID, ip, remotePort, 0, out error);
		//receiveData ();
		//Debug.Log (error);

	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < connections.Length; i++)
		{
            if (isConnected) {
                sendData();
            }
			receiveData();
		}
	}

	void sendData()
	{
		byte error;

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		int action = (Input.GetButton("Fire1")) ? 1 : 0;
		MovementActionMessage message = new MovementActionMessage(moveHorizontal, moveVertical, action, "The enemy");

		byte[] messageArray = message.toByteArray();

		NetworkTransport.Send(hostID, connectionID, myReliableChannelID, messageArray, messageArray.Length, out error);
	}

	void receiveData()
	{
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
				break;
			case NetworkEventType.ConnectEvent:    //2
				Debug.Log("<color=green>Connection Found</color>");
                Debug.Log("Host ID: " + hostID);
                Debug.Log("Connection ID: " + connectionID);
                Debug.Log("RecHost ID: " + recHostID);
                Debug.Log("RecConnection ID: " + recConnectionID);
				//myConnectionID
				if ((recHostID == hostID) && (currentConnections < maxConnections))
				{
					//my active connect request approved
					Debug.Log("<color=green>Successfully Connected</color>");
					connections[currentConnections] = connectionID;
					currentConnections++;
                    isConnected = true;
				}
				else
				{
					//somebody else connect to me
					Debug.LogWarning("Connection Rejected");
				}
				Debug.Log(currentConnections);
				break;
			case NetworkEventType.DataEvent:       //3
				Debug.Log("Data");
				MovementActionMessage message = new MovementActionMessage(recBuffer);
                Debug.Log("X: " + message.getX());
                Debug.Log("Y: " + message.getY());
                Debug.Log("Action: " + message.getAction());
                Debug.Log("ID: " + message.getID());


				GameObject opponent = GameObject.Find("Opponent");
				Debug.Log(opponent);
                if (idSet == false)
                {
                    PlayerIDController idController = opponent.GetComponent<PlayerIDController>();
                    idController.text = message.getID();
                    idController.messagePermanent = true;
                    idSet = true;
                }
				Done_PlayerController script = opponent.GetComponent<Done_PlayerController>();
				script.Move(message.getX() * (-1), message.getY() * (-1));
				script.executeAction(message.getAction());
				break;
			case NetworkEventType.DisconnectEvent: //4
				Debug.Log("Disconnection");
				//myConnectionID
				if (hostID == connectionID)
				{
					//cannot connect by some reason see error
					Debug.LogWarning("Error: Lost connection.");
				}
				else
				{
					//one of the established connection has been disconnected
					Debug.Log("Disconnected");
				}
				currentConnections--;
				break;
		}


	}

	void onApplicationQuit()
	{
		byte error;

		NetworkTransport.Disconnect(hostID, connectionID, out error);

		Debug.Log("disconnected");
		//Debug.Log (error);
	}
}