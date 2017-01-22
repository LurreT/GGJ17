using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Cannon : MonoBehaviour {

	[HideInInspector]
	public Vector3 target;
	public Animator fishAnim;

	/// <param name="prefab">The unit to instantiate from (duplicate) when firing.</param>
	/// <param name="impactAfter">Number of seconds for when object is supposed to reach <see cref="target"/>.</param>
	public void FireAt(SelfDestructUnit prefab, float impactAfter) {

		SelfDestructUnit clone = Instantiate(prefab, transform.position, Quaternion.identity);

		clone.delay = impactAfter;
		clone.target = target;
		// Add randomization
		clone.target += Random.Range(0, 360).FromDegrees(Random.Range(0, 8)).xzy(0);

		// Get components
		var body = clone.GetComponent<Rigidbody>();

		// Calculate velocity
		var delta = clone.target - clone.transform.position;
		body.velocity = VectorHelper.CalculateVelocity(delta, impactAfter / Time.fixedDeltaTime);

		clone.transform.forward = body.velocity.normalized;

		//fish launch anim
		fishAnim.SetTrigger("Launch");
	}

#if UNITY_EDITOR

	private void OnDrawGizmosSelected() {

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

	//void UpdatePredictionLine() {
	//	predictionLine.SetVertexCount(180);
	//	for (int i = 0; i < 180; i++) {
	//		Vector3 posN = GetTrajectoryPoint(spawnTarget.position, startingVelocity, i, Physics.gravity);
	//		predictionLine.SetPosition(i, posN);
	//	}
	//}

	Vector3 GetTrajectoryPoint(Vector3 startingPosition, Vector3 initialVelocity, float timestep) {
		float physicsTimestep = Time.fixedDeltaTime;
		Vector3 stepVelocity = physicsTimestep * initialVelocity;

		//Gravity is already in meters per second, so we need meters per second per second
		Vector3 stepGravity = physicsTimestep * physicsTimestep * Physics.gravity;

		return startingPosition + (timestep * stepVelocity) + (((timestep * timestep + timestep) * stepGravity) / 2.0f);
	}
#endif

}
