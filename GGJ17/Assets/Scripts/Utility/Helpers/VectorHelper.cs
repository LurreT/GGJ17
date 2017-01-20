using UnityEngine;
using System.Collections;

public class VectorHelper {

	public static Vector2 Average(params Vector2[] vectors) {
		if (vectors.Length == 0)
			return Vector2.zero;

		Vector2 total = Vector2.zero;
		foreach (var vec in vectors) {
			total += vec;
		}
		
		return total / vectors.Length;
	}

	public static Vector3 Average(params Vector3[] vectors) {
		if (vectors.Length == 0)
			return Vector3.zero;

		Vector3 total = Vector3.zero;
		foreach (var vec in vectors) {
			total += vec;
		}
		
		return total / vectors.Length;
	}
}

