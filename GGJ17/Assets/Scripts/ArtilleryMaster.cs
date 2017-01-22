using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ArtilleryMaster : MonoBehaviour {

	public AudioSource audioPrefab;
	public float echoTime = 2.5f;
	public bool timeByBPM;
	[HideInInspector]
	public int bpm = 120;
	public float bps { get { return timeByBPM ? bpm / 60f : 1; } }

	[HideInInspector]
	public List<SelfDestructUnit> units;
	[HideInInspector]
	public List<Cannon> spawners;
	[HideInInspector]
	public List<SingleStrike> strikes = new List<SingleStrike>();

	public bool isPlaying { get; private set; }

	private void Start() {
		StartSongShooting(3);
	}

	public void StartSongShooting(float startDelay = 0) {
		if (isPlaying) throw new System.Exception("A song is already playing from this object!");
		StartCoroutine(SongShootingRoutine(new List<SingleStrike>(strikes), startDelay));
	}

	public void StopSongShooting() {
		isPlaying = false;
	}

	private IEnumerator SongShootingRoutine(List<SingleStrike> strikes, float startDelay = 0) {

		// Import from all collections
		var count = strikes.Count;
		for (int i=count-1; i>=0; i--) {
			
			var spawnerInStrikeIndex = strikes[i].spawnerIndex;
			var spawnerInStrike = (spawnerInStrikeIndex>=0&&spawnerInStrikeIndex<spawners.Count) ? spawners[spawnerInStrikeIndex] : null;

			if (spawnerInStrike != null && spawnerInStrike is CannonCollection) {
				// Add all from it.
				foreach (var strikeInCollection in (spawnerInStrike as CannonCollection).strikes) {
					strikes.Add(new SingleStrike {
						timestamp = strikes[i].timestamp + strikeInCollection.timestamp,
						flytime = strikeInCollection.flytime,
						spawnerIndex = strikeInCollection.spawnerIndex,
						unitIndex = strikeInCollection.unitIndex,
					});
				}
				strikes.RemoveAt(i);
			}
		}

		// Give all references to their objects
		strikes.ForEach(s => {
			s.spawner = s.spawnerIndex >= 0 && s.spawnerIndex < spawners.Count ? spawners[s.spawnerIndex] : null;
			s.unit = s.unitIndex >= 0 && s.unitIndex < units.Count ? units[s.unitIndex] : null;

			// Randomized options
			while (s.spawnerIndex == -1 && (s.spawner == null || s.spawner is CannonCollection)) s.spawner = spawners[Random.Range(0, spawners.Count)];
			if (s.unitIndex == -1) s.unit = units[Random.Range(0, units.Count)];
		});

		// Clean list from invalid strikes
		strikes.RemoveAll(s => s.unit == null || s.spawner == null);

		// Let's go
		var audio = Instantiate(audioPrefab);
		audio.loop = false;
		audio.playOnAwake = false;

		yield return new WaitForSeconds(startDelay);

		audio.Play();
		isPlaying = true;

		float start = Time.time;

		while (strikes.Count > 0 && isPlaying) {
			// Spawn all that needs to and remove them at the same time.
			strikes.RemoveAll(s => {
				if (Time.time - start >= (s.timestamp - s.flytime)/bps) {
					// Your time has come
					s.spawner.FireAt(s.unit, s.flytime/bps);
					return true;
				} else return false;
			});

			if (strikes.Count == 0) break;

			// Wait for next strike
			// PRETTY INEFFICIENT SYSTEM.
			// CHECKS EVERY STRIKE EACH FRAME
			yield return new WaitUntil(() => !isPlaying || strikes.FindIndex(s => Time.time - start >= (s.timestamp - s.flytime)/bps) != -1);
		}

		// should start echo effect?
		yield return new WaitWhile(() => isPlaying && audio.clip.length - audio.time > echoTime);

		isPlaying = false;

		var echo = audio.gameObject.AddComponent<AudioEchoFilter>();

		// Fade out sound
		start = Time.unscaledTime;
		while (Time.unscaledTime - start < echoTime * 0.5f) {
			yield return new WaitForEndOfFrame();
			audio.volume = 1 - (Time.unscaledTime - start) / (echoTime * 0.5f);
		}

		audio.volume = 0;

		// Fade out echo
		start = Time.time;
		while (Time.time - start < echoTime) {
			yield return new WaitForEndOfFrame();
			echo.wetMix = 1 - (Time.time - start) / echoTime;
		}

		Destroy(audio.gameObject);
	}

	[System.Serializable]
	public class SingleStrike {
		public int spawnerIndex;
		public int unitIndex;
		public float timestamp;
		public float flytime;

		[System.NonSerialized]
		public Cannon spawner;
		[System.NonSerialized]
		public SelfDestructUnit unit;
	}
}
