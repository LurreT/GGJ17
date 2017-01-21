using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timescaler : MonoBehaviour {

	[Range(0,2)]
	public float scale = 1;

	private void OnValidate() {
		Time.timeScale = scale;
		if (scale > float.Epsilon)
		Time.fixedDeltaTime = 0.02f * scale;
	}
	
}
