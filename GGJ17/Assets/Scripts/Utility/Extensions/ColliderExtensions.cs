using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace ExtensionMethods {
	public static class ColliderExstensions {
		public static GameObject GetMainObject(this Collider col) {
			return col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;
		}
	}
}