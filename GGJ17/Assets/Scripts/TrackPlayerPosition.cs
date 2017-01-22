using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayerPosition : MonoBehaviour {

	public float saveTime;

	private List<SavedPos> pos = new List<SavedPos>();

	private void LateUpdate() {
		pos.RemoveAll(p => Time.time - p.timestamp > saveTime);
		pos.Add(new SavedPos { timestamp = Time.time, position = transform.position });
	}

	public Vector3 GetLastPosition() {
		if (pos.Count == 0) return transform.position;
		return pos[0].position;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if (!UnityEditor.EditorApplication.isPlaying) return;

		Vector3 last = transform.position;
		Gizmos.color = Color.green;
		for (int i=pos.Count-1; i>=0; i--) {
			Vector3 next = pos[i].position;
			Gizmos.DrawLine(last, next);
			last = next;
		}
	}
#endif

	private struct SavedPos {
		public float timestamp;
		public Vector3 position;
	}

}
