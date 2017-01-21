using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructUnit : MonoBehaviour {

	[HideInInspector]
	public float delay;
	private float timestamp;

	private static float lowestOff = Mathf.Infinity;
	private static float highestOff = Mathf.NegativeInfinity;

	private void Awake() {
		timestamp = Time.time;
	}

	private void Update() {
		if (Time.time >= timestamp + delay) {
			// Debugging
			float took = Time.time - timestamp;
			float diff = Mathf.Abs(FindObjectOfType<Cannon>().timeToFly - took);

			if (diff < lowestOff) lowestOff = diff;
			if (diff > highestOff) highestOff = diff;

			print("I LIVED IN " + (Time.time - timestamp) + " SECONDS\nLOWEST ERROR = " + lowestOff + "\nHIGHEST ERROR = " + highestOff);

			// Do it. Kill it. Good goood
			SendMessage("OnSelfDestruct", SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
	}
}
