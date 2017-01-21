using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	float shake = 0;
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1;
	Vector3 firstPos;

	void Start(){
		firstPos = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (shake > 0) {
			transform.position = firstPos + Random.insideUnitSphere * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;
		} else {
			shake = 0;
			transform.position = firstPos;
		}
	}
	public void Shake(float shakeTime){
		firstPos = transform.position;
		shake = shakeTime;
	}
}
