using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScaleAudioWithTimeScale : MonoBehaviour {

	private float startPitch = 1;
	private AudioSource source;

	private void Awake() {
		source = GetComponent<AudioSource>();
	}

	private void Start() {
		startPitch = source.pitch;
	}

	private void Update() {
		source.pitch = startPitch * Time.timeScale;
	}
}
