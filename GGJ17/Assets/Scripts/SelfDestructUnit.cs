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

#if UNITY_EDITOR
	private static float lowestOff = Mathf.Infinity;
	private static float highestOff = Mathf.NegativeInfinity;
#endif

	private void Awake() {
		timestamp = Time.time;
		body = GetComponent<Rigidbody>();
		shadowClone = Instantiate(shadowPrefab, target + Vector3.up * shadowProjOffset, shadowPrefab.transform.rotation);
	}

	private void Update() {
		if (Time.time >= timestamp + delay) {
#if UNITY_EDITOR
			// Debugging
			float took = Time.time - timestamp;
			float diff = Mathf.Abs(delay - took);

			if (diff < lowestOff) lowestOff = diff;
			if (diff > highestOff) highestOff = diff;

			print("I LIVED IN " + (Time.time - timestamp) + " SECONDS\nLOWEST ERROR = " + lowestOff + "\nHIGHEST ERROR = " + highestOff);
#endif

			// Do it. Kill it. Good goood
			transform.position = target;
			Destroy(shadowClone.gameObject);
			Destroy(gameObject);
			Instantiate (shockWave, new Vector3 (transform.position.x, 0, transform.position.z), Quaternion.identity);
			SoundEffectSpawner.PlaySoundEffectAt(transform.position, soundOnDeath);
		} else {
			// Rotate
			transform.forward = body.velocity;

			float percent = Mathf.InverseLerp(timestamp, timestamp + delay, Time.time);
			shadowClone.farClipPlane = (percent+.5f) * shadowProjOffset * 2;
		}
	}
}