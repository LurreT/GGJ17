using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour {

	PlayerHealth ph;
	public float selfDestructIn;

	void Start(){
		Destroy(transform.root.gameObject, selfDestructIn);
	}

	void OnTriggerEnter(Collider col){
		ph = col.gameObject.GetComponent<PlayerHealth> ();
		if (ph != null) {
			ph.Damage ();
		}
	}
	
}
