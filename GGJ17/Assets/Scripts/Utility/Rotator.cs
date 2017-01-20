using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public Vector3 delta;
	public Space relativeTo;
	
	void Update () {
		if (relativeTo == Space.Self)
			transform.localEulerAngles += delta * Time.deltaTime;
		else
			transform.eulerAngles += delta * Time.deltaTime;
	}
}
