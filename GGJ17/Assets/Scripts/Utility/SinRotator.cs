using UnityEngine;
using System.Collections;

public class SinRotator : MonoBehaviour {

	public float amplitude = 1;
	public float waveLength = 1;
	public bool localRotation = true;
	[Header("Axis")]
	public bool x = false;
	public bool y = true;
	public bool z = false;

	private Quaternion rot;
	private float invWaveLength;

#if UNITY_EDITOR
	private bool _localRotation;
	void OnValidate() {
		if (Application.isPlaying) {
			if (localRotation != _localRotation) {
				rot = localRotation ? transform.localRotation : transform.rotation;
			}
		}
		_localRotation = localRotation;
		invWaveLength = 1 / waveLength;
	}
#endif

	void Start() {
		rot = localRotation ? transform.localRotation : transform.rotation;
		invWaveLength = 1 / waveLength;
#if UNITY_EDITOR
		_localRotation = localRotation;
#endif
	}

void Update() {
		float v = Mathf.Sin(Time.time * Mathf.PI * invWaveLength * 0.5f) * amplitude;
		rot = Quaternion.Euler(x?v:0,y?v:0,z?v:0);

		if (localRotation)
			transform.localRotation = rot;
		else
			transform.rotation = rot;
	}
}
