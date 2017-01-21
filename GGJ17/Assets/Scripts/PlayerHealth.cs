using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public int health;
	public List<Image> heart;
	public CameraShake camShake;

	void Start(){
//		heart = new List<Image> ();
		for (int i = 0; i < health; i++) {
			heart[i].gameObject.SetActive(true);
		}
	}

	public void Damage(){
		health--;
		if (health == 0) {
			Death ();
		}
		heart [health].gameObject.SetActive (false);
		camShake.Shake(1);
	}
	void Death(){
		GetComponent<PlayerMovement> ().enabled = false;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
	}
}
