using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectSpawner : MonoBehaviour {

	public AudioSource prefab;
	public AudioClip sound_impact1;
	public AudioClip sound_impact2;

	public static void PlaySoundEffectAt(Vector3 position, SoundEffect effect) {
		var spawner = FindObjectOfType<SoundEffectSpawner>();
		if (spawner == null)
			throw new System.Exception("No sound effect spawner found! Please make one, dear.");

		AudioClip clip = null;
		if (effect == SoundEffect.Impact1) clip = spawner.sound_impact1;
		else if (effect == SoundEffect.Impact2) clip = spawner.sound_impact2;

		if (clip == null) return;
		var clone = Instantiate(spawner.prefab, position, Quaternion.identity);
		clone.clip = clip;

		clone.volume = Random.Range(0.8f, 1);
		clone.pitch = Random.Range(0.9f, 1.1f);

		clone.Play();
		Destroy(clone.gameObject, clone.clip.length);
	}

	public enum SoundEffect {
		None,
		Impact1,
		Impact2
	}
}
