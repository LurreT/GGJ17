using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
		Physics.IgnoreLayerCollision (8, 9);
	}

	public void Damage(){
		health--;
		if (health == 0) {
			Death();
		} else if (health < 0)
			return;

		heart [health].gameObject.SetActive (false);

		camShake = FindObjectOfType<CameraShake>();
		if (camShake != null) camShake.Shake(.5f);
		PlayerDamageIndicator.IndicateSomeDamageYao();
	}
	void Death(){
		GetComponent<PlayerMovement> ().enabled = false;
//		GetComponent<PlayerMovement> ().anim.enabled = false;
		GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		FindObjectOfType<ArtilleryMaster>().StopSongShooting();
		StartCoroutine (AfterDeath ());
		GetComponent<PlayerMovement> ().anim.SetBool ("Dead", true);
	}
	IEnumerator AfterDeath(){
		yield return new WaitForSeconds (3);
		SwitchScene.GotoScene("EndScene");
	}
}
