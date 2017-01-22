using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SelfDestructUnit : MonoBehaviour {

	public SoundEffectSpawner.SoundEffect soundOnDeath;
	public Projector shadowPrefab;
	public float shadowProjOffset = 5;
	private Projector shadowClone;

	[System.NonSerialized]
	public Vector3 target;
	[System.NonSerialized]
	public float delay;

	private float timestamp;
	private Rigidbody body;

	public GameObject shockWave;

	private void Awake() {
		timestamp = Time.time;
		body = GetComponent<Rigidbody>();
	}

	private void Start() {
		shadowClone = Instantiate(shadowPrefab, target + Vector3.up * shadowProjOffset, shadowPrefab.transform.rotation);
	}

	private void Update() {
		if (Time.time >= timestamp + delay) {

			// Do it. Kill it. Good goood
			transform.position = target;
			Destroy(shadowClone.gameObject);
			Destroy(gameObject);
			Instantiate (shockWave, new Vector3 (transform.position.x, 0, transform.position.z), Quaternion.identity);
			SoundEffectSpawner.PlaySoundEffectAt(transform.position, soundOnDeath);

			var shake = FindObjectOfType<CameraShake>();
			if (shake != null) shake.Shake(0.05f);
		} else {
			// Rotate
			transform.forward = body.velocity;

			float percent = Mathf.InverseLerp(timestamp, timestamp + delay, Time.time);
			shadowClone.farClipPlane = (percent+.5f) * shadowProjOffset * 2;
		}
	}
}