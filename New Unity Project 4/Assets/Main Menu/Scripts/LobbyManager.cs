using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NetworkConfiguration.networkManager = new NetworkController2();
		NetworkConfiguration.allowConnections = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		NetworkConfiguration.networkManager.receiveData();
	}
}
