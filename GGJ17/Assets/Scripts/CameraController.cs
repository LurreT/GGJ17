using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
	/*
		A programmer once stated that when it comes to comments in code,
		your variable and function names should be enough.
		With the exception for complex forumlas..
		Not everyone knows pythagoras theorym.

		Well, everyone does, but you get the point.

		My descriptive names are at least better than foo bar x)
	*/

	public Transform player;
	[Range(0,1)]
	public float scaleForCameraFollowingMultiplierIfSetToOneItFollowsDirectlyIfSetToAHalfItOnlyFollowsHalfWayItIsSimple = 1;
	public float speedInWichTheCameraFollowsThePlayerSoThatItInterpolatesInANiceAndSmoothFashion = 5;

	private Vector3 offsetFromPlayer;
	private Vector3 centerPosition;

	private void Start() {
		offsetFromPlayer = transform.position - player.position;
		centerPosition = transform.position;
	}

	private void LateUpdate() {
		var ultimateTarget = player.position + offsetFromPlayer;
		var scaledTarget = Vector3.Lerp(centerPosition, ultimateTarget, scaleForCameraFollowingMultiplierIfSetToOneItFollowsDirectlyIfSetToAHalfItOnlyFollowsHalfWayItIsSimple);
		var smoothedTarget = Vector3.Lerp(transform.position, scaledTarget, Time.deltaTime * speedInWichTheCameraFollowsThePlayerSoThatItInterpolatesInANiceAndSmoothFashion);

		transform.position = smoothedTarget;
	}

}
