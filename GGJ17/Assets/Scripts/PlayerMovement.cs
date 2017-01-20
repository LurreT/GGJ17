using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	Rigidbody rb;
	public float speed;
	public float jumpForce;

	void Start(){
		rb = GetComponent<Rigidbody> ();

	}

	
	// Update is called once per frame
	void FixedUpdate () {
		print (Input.GetAxis ("Horizontal"));

		if (rb.velocity.z < speed) {
			rb.velocity += new Vector3 (0, 0, Input.GetAxis ("Vertical")) * speed;
		} else {
			rb.velocity = new Vector3 (0, 0, Input.GetAxis ("Vertical")) * speed;
		}
		if (rb.velocity.x < speed) {
			rb.velocity += new Vector3 (Input.GetAxis ("Horizontal"), 0,0) * speed;
		}

		print (Input.GetAxis ("Jump"));

		if (Input.GetAxis ("Jump") > 0.5f) {
			print ("yas");
			rb.AddForce (Vector3.up * jumpForce);
		}

	}
}
