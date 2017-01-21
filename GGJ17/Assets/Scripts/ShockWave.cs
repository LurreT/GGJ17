using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour {

	PlayerHealth ph;
	public float selfDestructIn;

	void Start(){
		StartCoroutine (SelfDestruct ());
	}

	void OnTriggerEnter(Collider col){
		ph = col.gameObject.GetComponent<PlayerHealth> ();
		if (ph != null) {
			ph.Damage ();
		}
	}

	IEnumerator SelfDestruct(){
		yield return new WaitForSeconds (selfDestructIn);
		Destroy (gameObject);
	}
}
