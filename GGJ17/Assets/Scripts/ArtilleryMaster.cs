using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ArtilleryMaster : MonoBehaviour {

	public AudioSource audioPrefab;
	public float echoTime = 2.5f;

	[HideInInspector]
	public List<SelfDestructUnit> units;
	[HideInInspector]
	public List<Cannon> spawners;
	[HideInInspector]
	public List<SingleStrike> strikes = new List<SingleStrike>();

	public bool isPlaying { get; private set; }

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (!isPlaying)
				StartSongShooting();
			else
				StopSongShooting();
		}
	}

	public void StartSongShooting() {
		if (isPlaying) throw new System.Exception("A song is already playing from this object!");
		StartCoroutine(SongShootingRoutine(new List<SingleStrike>(strikes)));
	}

	public void StopSongShooting() {
		isPlaying = false;
	}

	private IEnumerator SongShootingRoutine(List<SingleStrike> strikes) {
		// Give all references to their objects
		strikes.ForEach(s => {
			s.spawner = spawners[s.spawnerIndex];
			s.unit = units[s.unitIndex];
		});
		
		var audio = Instantiate(audioPrefab);
		audio.loop = false;
		audio.playOnAwake = false;
		audio.Play();
		isPlaying = true;

		float start = Time.unscaledTime;

		while (strikes.Count > 0 && isPlaying) {
			// Spawn all that needs to and remove them at the same time.
			strikes.RemoveAll(s => {
				if (Time.unscaledTime - start >= s.timestamp - s.flytime) {
					// Your time has come
					s.spawner.FireAt(s.unit, s.flytime);
					return true;
				} else return false;
			});

			if (strikes.Count == 0) break;

			// Wait for next strike
			// PRETTY INEFFICIENT SYSTEM.
			// CHECKS EVERY STRIKE EACH FRAME
			yield return new WaitUntil(() => !isPlaying || strikes.FindIndex(s => Time.unscaledTime - start >= s.timestamp - s.flytime) != -1);
		}

		// should start echo effect?
		yield return new WaitWhile(() => isPlaying && audio.clip.length - audio.time > echoTime);

		var echo = audio.gameObject.AddComponent<AudioEchoFilter>();

		// Fade out sound
		start = Time.unscaledTime;
		while (Time.unscaledTime - start < echoTime * 0.5f) {
			yield return new WaitForEndOfFrame();
			audio.volume = 1 - (Time.unscaledTime - start) / (echoTime * 0.5f);
		}

		audio.volume = 0;

		// Fade out echo
		start = Time.unscaledTime;
		while (Time.unscaledTime - start < echoTime) {
			yield return new WaitForEndOfFrame();
			echo.wetMix = 1 - (Time.unscaledTime - start) / echoTime;
		}

		isPlaying = false;
		Destroy(audio.gameObject);
	}

	[System.Serializable]
	public class SingleStrike {
		public int spawnerIndex;
		public int unitIndex;
		public float timestamp;
		public float flytime;

		public Cannon spawner;
		public SelfDestructUnit unit;
	}
}
