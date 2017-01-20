using UnityEngine;
using System.Collections;

public class SinMover : MonoBehaviour {

	public float amplitude = 1;
	public float waveLength = 1;
	public bool localPosition = true;
	[Header("Axis")]
	public bool x = false;
	public bool y = true;
	public bool z = false;
	
	private float invWaveLength;

	void Start() {
		invWaveLength = 1 / waveLength;
	}

	void Update() {
		Vector3 pos = localPosition ? transform.localPosition : transform.position;

		float v = Mathf.Sin(Time.time * Mathf.PI * invWaveLength * 0.5f) * amplitude;
		if (x) pos.x = v;
		if (y) pos.y = v;
		if (z) pos.z = v;

		if (localPosition)
			transform.localPosition = pos;
		else
			transform.position = pos;
	}
}
