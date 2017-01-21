using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {
	
	public float timeToFly = 5;
	
	/// <param name="targetPosition">Position where object is supposed to land in world space.</param>
	/// <param name="impactAfter">Number of seconds for when object is supposed to reach <paramref name="targetPosition"/>.</param>
	public void FireAt(SelfDestructUnit prefab, Vector3 targetPosition, float impactAfter) {

		SelfDestructUnit clone = Instantiate(prefab, transform.position, Quaternion.identity);

		clone.delay = impactAfter;

		// Get components
		var body = clone.GetComponent<Rigidbody>();

		// Calculate velocity
		var delta = targetPosition - clone.transform.position;
//		body.velocity = VectorHelper.CalculateVelocity(delta, timeToFly / Time.fixedDeltaTime);
	}

}
