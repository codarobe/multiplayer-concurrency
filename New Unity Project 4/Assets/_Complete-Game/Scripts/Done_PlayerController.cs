using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Done_Boundary {
	public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour {
	public float speed;
	public float tilt;
	//public Done_Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float startingHealth = 100;
	private float playerHealth;
	private Slider healthSlider;

	private float nextFire;
	private bool isDead = false;

	public bool isLocalPlayer;

	private void Awake()
	{
		playerHealth = startingHealth;
		healthSlider = GameObject.Find(gameObject.name + "/HealthCanvas/HealthSlider").GetComponent<UnityEngine.UI.Slider>();
		healthSlider.value = playerHealth;
	}

	void Update() {
		if (Input.GetButton("Fire1"))
		{
			fire();
		}
	}

	bool fire() {
		if (Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play();
			return true;
		}
		return false;
	}

	void FixedUpdate()
	{
		healthSlider.value = playerHealth;
		if (isLocalPlayer) {
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			Move(moveHorizontal, moveVertical);
		}
		else {

		}
	}

	public void Move(float moveHorizontal, float moveVertical) {


		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody>().velocity = movement * speed;

		//GetComponent<Rigidbody>().position = new Vector3 (
			//Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
			//0.0f,
			//Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		//);

		if (isLocalPlayer) {
			GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
		}
		else {
			GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 180.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
		}
	}

	public void damagePlayer(float damage)
	{
		if (playerHealth - damage <= 0)
		{
			playerHealth = 0;
			isDead = true;
		}
		else
		{
			playerHealth -= damage;
		}
	}

	public bool isPlayerDead()
	{
		return isDead;
	}
	
	public void executeAction(int action) {
		switch (action) {
			case 1:
				fire();
				break;
			default:
				break;
		}
	}

}