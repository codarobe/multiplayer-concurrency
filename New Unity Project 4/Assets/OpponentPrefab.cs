using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPrefab : MonoBehaviour
{

	public GameObject opponent;
	public GameObject player;

	private void Start()
	{
		NetworkConfiguration.networkController.spawnPlayers();
	}

	private void Update()
	{
		NetworkConfiguration.networkController.receiveData();
	}
}
