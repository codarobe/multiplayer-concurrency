using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPrefab : MonoBehaviour
{

	public GameObject opponent;
	public GameObject player;

	public GameObject gameController;
	public GameObject players;

	private void Start()
	{
		Debug.Log("Start Spawning Players!");
		Dictionary<int, ClientNetworkManager> spawns = NetworkConfiguration.networkController.getSpawns();

		if (spawns.Count < 1)
		{
			Debug.LogWarning("Error: no spawn points designated!");
		}
		else
		{
			Debug.LogWarning(spawns);
		}

		foreach (KeyValuePair<int, ClientNetworkManager> kvp in spawns)
		{
			GameObject spawnPoint = GameObject.Find("SpawnPoints/spawn" + kvp.Key.ToString());
			if (spawnPoint != null)
			{
				Vector3 transform = spawnPoint.GetComponent<Transform>().position;
				Quaternion rotation = spawnPoint.GetComponent<Transform>().rotation;
				if (kvp.Value != null)
				{
					GameObject newPlayer = Instantiate(opponent, transform, rotation);
					newPlayer.transform.parent = players.GetComponent<Transform>();
					kvp.Value.player = newPlayer;
					PlayerIDController idController = newPlayer.GetComponent<PlayerIDController>();
					idController.text = kvp.Value.getIdentifier();
					idController.messagePermanent = true;
					
					Debug.Log("Spawned Opponent Player: " + kvp.Value.getIdentifier() + " at spawnpoint: " + kvp.Key);
				}
				else
				{
					GameObject newPlayer = Instantiate(player, transform, rotation);
					newPlayer.transform.parent = players.GetComponent<Transform>();
					NetworkConfiguration.networkController.localPlayer = newPlayer;
					PlayerIDController idController = newPlayer.GetComponent<PlayerIDController>();
					idController.text = NetworkConfiguration.playerName;
					idController.messagePermanent = true;

					Debug.Log("Spawned Local Player: " + NetworkConfiguration.playerName + " at spawnpoint: " + kvp.Key);
				}
			}
			else
			{
				Debug.LogWarning("Error: spawnpoint not found: spawn"+kvp.Key.ToString());
			}
			
		}
		Debug.Log("Players Spawned!");
	}

	private void Update()
	{
		Camera mainCamera = gameController.GetComponent<Camera>();
		Camera playerCamera = NetworkConfiguration.networkController.localPlayer.GetComponent<Camera>();
		if (mainCamera.enabled)
		{
			mainCamera.enabled = false;
			playerCamera.enabled = true;
		}
		NetworkConfiguration.networkController.broadcastMovementAction();
		NetworkConfiguration.networkController.receiveData();
	}

}
