using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Boundray
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    private float nextFire;
    public float speed;
    public float tilt;
    public Boundray boundray;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;

	public bool isLocalPlayer;

      void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            //GameObject clone = 
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //as GameObject;
        }
    }
    private void FixedUpdate()
    {
        if (isLocalPlayer) {
			float moveHorizontal = Input.GetAxis("Horizontal");
			float moveVertical = Input.GetAxis("Vertical");

			Move(moveHorizontal, moveVertical);
        }
        else {
            
        }
    }

	public void Move(float moveHorizontal, float moveVertical) {
		

        //Debug.Log(moveHorizontal);
        //Debug.Log(moveVertical);

		Vector3 movement= new Vector3(moveHorizontal, 0.0f, moveVertical);
		rb.velocity = movement * speed;

		rb.position = new Vector3
			(
				Mathf.Clamp(rb.position.x, boundray.xMin,boundray.xMax), 
				0.0f,
				Mathf.Clamp(rb.position.z,boundray.zMin,boundray.zMax)
			);

		rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x* -tilt);
	}
}
