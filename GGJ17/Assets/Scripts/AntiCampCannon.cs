using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class AntiCampCannon : Cannon {

	public TrackPlayerPosition camperTracker;

	private void Update() {
		target = camperTracker.GetLastPosition().SetY(0);
	}


#if UNITY_EDITOR

	private void OnDrawGizmosSelected() {
		if (camperTracker == null) return;

		target = camperTracker.GetLastPosition().SetY(0);

		UnityEditor.Handles.color = new Color(0, 1, 1);
		UnityEditor.Handles.ArrowCap(-1, target + Vector3.up, Quaternion.FromToRotation(Vector3.forward, Vector3.down), 1);
		UnityEditor.Handles.CircleCap(-1, target, Quaternion.FromToRotation(Vector3.forward, Vector3.up), 1);

		int timesteps = Mathf.FloorToInt(1.5f / Time.fixedDeltaTime);

		Vector3 vel = VectorHelper.CalculateVelocity(target-transform.position, timesteps);
		Vector3 last = transform.position;
		Gizmos.color = new Color(0, 1, 0, 0.5f);
		for (int i = 0; i < timesteps; i++) {
			vel += Physics.gravity * Time.fixedDeltaTime;
			Vector3 next = i == timesteps - 1
			?	target
			:	last + vel * Time.fixedDeltaTime;

			Gizmos.DrawLine(last, next);
			last = next;
		}
	}

	Vector3 GetTrajectoryPoint(Vector3 startingPosition, Vector3 initialVelocity, float timestep) {
		float physicsTimestep = Time.fixedDeltaTime;
		Vector3 stepVelocity = physicsTimestep * initialVelocity;

		//Gravity is already in meters per second, so we need meters per second per second
		Vector3 stepGravity = physicsTimestep * physicsTimestep * Physics.gravity;

		return startingPosition + (timestep * stepVelocity) + (((timestep * timestep + timestep) * stepGravity) / 2.0f);
	}
#endif

}
