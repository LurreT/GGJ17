using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnWebGL : MonoBehaviour {

	private void Awake() {
#if UNITY_WEBGL
		gameObject.SetActive(false);
#endif
	}
}
