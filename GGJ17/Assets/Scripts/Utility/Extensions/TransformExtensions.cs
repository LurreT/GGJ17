using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace ExtensionMethods {
	public static class TransformExitensions {
		/// <summary>
		/// Get the full hierarchy path of a transfrom.
		/// Recursive.
		/// </summary>
		public static string GetPath(this Transform self) {
			// Recursive

			if (self.parent == null)
				return self.name;

			return self.parent.GetPath() + "/" + self.name;
		}

		/// <summary>
		/// Is the transform selected in the heirarchy?
		/// </summary>
		public static bool IsSelected(this Transform self, bool includeParents = true) {
#if UNITY_EDITOR
			foreach (Transform t in UnityEditor.Selection.transforms) {
				if (t == self || (includeParents && self.IsChildOf(t))) return true;
			}
			return false;
#else
			return false;
#endif
		}
	}
}