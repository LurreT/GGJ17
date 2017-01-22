using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerDamageIndicator : MonoBehaviour {

	private Image gradientImage;
	public float duration = 0.5f;
	public float alphaMultiplier = 0.5f;
	private static float alpha;

	private void Awake() {
		gradientImage = GetComponent<Image>();
	}

	private void Update() {
		alpha = Mathf.Clamp01(alpha - Time.deltaTime / duration);
		if (gradientImage) {
			var c = gradientImage.color;
			c.a = alpha * alphaMultiplier;
			gradientImage.color = c;
		}
	}

	public static void IndicateSomeDamageYao() {
		alpha = 1;
	}

}
