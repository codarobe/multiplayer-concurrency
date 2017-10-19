using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private Done_GameController gameController;
  

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Done_GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag=="Enemy")
		{
			return;
		}

		GameObject parent = GameObject.Find("Players");

		// if collision with player
		if (gameObject.transform.IsChildOf(parent.transform))
		{
			// damage player and despawn if out of health
			Done_PlayerController playerController = gameObject.GetComponent<Done_PlayerController>();
			playerController.damagePlayer(10);
			if (playerController.isPlayerDead())
			{
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				if (!playerController.isLocalPlayer)
				{
					gameController.AddScore(scoreValue);
				}
				Destroy(gameObject);
				//gameController.GameOver();
			}
		}
		else
		{
			Destroy(gameObject);
		}
		
		if (other.transform.IsChildOf(parent.transform))
		{
			// damage player and despawn if out of health
			Done_PlayerController playerController = other.gameObject.GetComponent<Done_PlayerController>();
			playerController.damagePlayer(10);
			if (playerController.isPlayerDead())
			{
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				if (!playerController.isLocalPlayer)
				{
					gameController.AddScore(scoreValue);
				}
				Destroy(other.gameObject);
				//gameController.GameOver();
			}
		}
		else
		{
			Destroy(other.gameObject);
		}

//		else if (other.tag == "Player")
//		{
//			Done_PlayerController playerController = other.GetComponent<Done_PlayerController>();
//			playerController.damagePlayer(10);
//			if (playerController.isPlayerDead())
//			{
//				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
//				Destroy(other.gameObject);
//				//gameController.GameOver();
//			}
//		}

        //gameController.AddScore(scoreValue);
		//Destroy (other.gameObject);
		//Destroy (gameObject);
	}
}