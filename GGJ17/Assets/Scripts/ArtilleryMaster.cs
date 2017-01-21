using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMaster : MonoBehaviour {

	[HideInInspector]
	public List<SelfDestructUnit> units;
	[HideInInspector]
	public List<Cannon> spawners;
	[HideInInspector]
	public List<SingleStrike> strikes = new List<SingleStrike>();

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			// Get point of mouse in world space
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				StartSongShooting();
			}
		}
	}

	public void StartSongShooting() {
		StartCoroutine(SongShootingRoutine(new List<SingleStrike>(strikes)));
	}

	public void StopSongShooting() {
		StopAllCoroutines();
	}

	private IEnumerator SongShootingRoutine(List<SingleStrike> strikes) {
		// Give all references to their objects
		strikes.ForEach(s => {
			s.spawner = GetStrikeSpawner(s.spawnerName);
			s.unit = GetStrikeUnit(s.unitName);
		});

		float start = Time.unscaledTime;

		while (strikes.Count > 0) {
			// Spawn all that needs to and remove them at the same time.
			strikes.RemoveAll(s => {
				if (Time.unscaledTime - start >= s.timestamp - s.flytime) {
					// Your time has come
					s.spawner.FireAt(s.unit, s.flytime);
					return true;
				} else return false;
			});
			
			if (strikes.Count == 0) break;

			// Wait for next
			// PRETTY INEFFICIENT SYSTEM.
			// CHECKS EVERY STRIKE EACH FRAME
			yield return new WaitUntil(() => strikes.FindIndex(s=> Time.unscaledTime - start >= s.timestamp - s.flytime) != -1);
		}
	}

	public Cannon GetStrikeSpawner(string name) {
		return spawners.Find(c => c.name == name);
	}

	public SelfDestructUnit GetStrikeUnit(string name) {
		return units.Find(u => u.name == name);
	}

	[System.Serializable]
	public class SingleStrike {
		public string spawnerName;
		public string unitName;
		public float timestamp;
		public float flytime;

		public Cannon spawner;
		public SelfDestructUnit unit;
	}
}
