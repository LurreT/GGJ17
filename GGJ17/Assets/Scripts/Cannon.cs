using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	public Vector3 target;

	/// <param name="prefab">The unit to instantiate from (duplicate) when firing.</param>
	/// <param name="impactAfter">Number of seconds for when object is supposed to reach <see cref="target"/>.</param>
	public void FireAt(SelfDestructUnit prefab, float impactAfter) {

		SelfDestructUnit clone = Instantiate(prefab, transform.position, Quaternion.identity);

		clone.delay = impactAfter;
		clone.target = target;

		// Get components
		var body = clone.GetComponent<Rigidbody>();

		// Calculate velocity
		var delta = target - clone.transform.position;
		body.velocity = VectorHelper.CalculateVelocity(delta, impactAfter / Time.fixedDeltaTime);

		clone.transform.forward = body.velocity.normalized;
	}

}
