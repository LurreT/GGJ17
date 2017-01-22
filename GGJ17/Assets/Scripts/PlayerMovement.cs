using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	Rigidbody rb;
	public float speed;
	public float jumpForce;
	public float dashForce;
	bool isGrounded;
	bool justJumped;
	bool justDashed;
	public float jumpDelay;
	public float dashDelay;
	public float groundedHeight;
	public float drag;
	public Animator anim;

	void Start(){
		rb = GetComponent<Rigidbody> ();
	}

	void Update(){
		if (Physics.Raycast (transform.position, -Vector3.up, groundedHeight)) {
			if (!justJumped) {
				isGrounded = true;
				anim.SetBool ("isGrounded", true);
			}
		} else {
			isGrounded = false;
			anim.SetBool ("isGrounded", false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (rb.velocity.z < speed && rb.velocity.z > -speed) {
			rb.velocity += new Vector3 (0, 0, Input.GetAxis ("Vertical")) * speed;
		} else {
//			rb.velocity = new Vector3 (0, 0, Input.GetAxis ("Vertical")) * speed;

		}
		if (rb.velocity.x < speed && rb.velocity.x > -speed) {
			rb.velocity += new Vector3 (Input.GetAxis ("Horizontal"), 0,0) * speed;
		} else {
//			rb.velocity = new Vector3 (Input.GetAxis ("Horizontal"), 0,0) * speed;

//			
		}

		//Drag
		if(rb.velocity.z < 0){
//			rb.velocity += new Vector3 (0, 0, drag);
			rb.AddForce (0, 0, drag);
		}else{
//			rb.velocity += new Vector3 (0, 0, -drag);
			rb.AddForce (0, 0, -drag);
		}
		if(rb.velocity.x < 0){
//			rb.velocity += new Vector3 (drag,0,0);
			rb.AddForce (drag,0,0);
		}else{
//			rb.velocity += new Vector3 (-drag,0,0);
			rb.AddForce (-drag,0,0);
		}


		if (Input.GetAxis ("Horizontal") == 0 && Input.GetAxis ("Vertical") == 0 && !justDashed) {
			rb.velocity = new Vector3 (0,rb.velocity.y,0);
		}


		Debug.DrawRay (transform.position, -Vector3.up * groundedHeight, Color.red);


		if (!isGrounded) {
//			rb.velocity += 	
		}
		Debug.Log ("isGrounded: " + isGrounded);

		if (Input.GetAxis ("Jump") > 0.5f && isGrounded) {
			rb.AddForce (Vector3.up * jumpForce);
			justJumped = true;
			isGrounded = false;
			anim.SetBool ("isGrounded", false);
			StartCoroutine (JumpDelay ());
		}
		if (Input.GetAxis ("Submit") > 0 && !justDashed) {
			rb.AddForce (transform.forward * dashForce);
			justDashed = true;
			anim.SetTrigger ("Dash");
			StartCoroutine (DashDelay ());
		}

		if (Input.GetAxis ("Horizontal") > 0 || Input.GetAxis ("Vertical") > 0 || Input.GetAxis ("Horizontal") < 0 || Input.GetAxis ("Vertical") < 0) {
			transform.forward = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		}


		anim.SetFloat ("Blend", rb.velocity.magnitude / speed);

		if (Input.GetAxis("Cancel") > 0) {
			GameObject.Find ("PauseMenu").GetComponent<Menu>().Pause();
		}
	}
	IEnumerator JumpDelay(){
		yield return new WaitForSeconds (jumpDelay);
		justJumped = false;
	}
	IEnumerator DashDelay(){
		yield return new WaitForSeconds (dashDelay);
		justDashed = false;
	}
}
