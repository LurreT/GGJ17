using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour {

	PlayerHealth ph;

	void OnTriggerEnter(Collider col){
		ph = col.gameObject.GetComponent<PlayerHealth> ();
		if (ph != null) {
			ph.Damage ();
		}
	}
}
