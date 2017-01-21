using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public int health;
	public Image heart;


	void Start(){
		
	}

	public void Damage(){
		health--;
		if (health == 0) {
			Death ();
		}
	}
	void Death(){
		GetComponent<PlayerMovement> ().enabled = false;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
	}
}
