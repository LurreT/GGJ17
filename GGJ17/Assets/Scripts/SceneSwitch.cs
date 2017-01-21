using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchScene : MonoBehaviour {

	private Image blackness;
	private float currentFade;
	private float duration;
	private State state = State.Invalid;
	private string scene;
	private AsyncOperation loading;
	private Action callback;

	private void Update() {
		if (state == State.Hide) {
			currentFade = Mathf.Clamp01(currentFade + Time.deltaTime / duration);
			blackness.color = new Color(0, 0, 0, currentFade);

			if (Mathf.Approximately(currentFade, 1f)) {
				blackness.color = new Color(0, 0, 0, currentFade = 1);
				// Goto next phase
				state = State.Loading;
				loading = SceneManager.LoadSceneAsync(scene);
			}

		} else if (state == State.Loading) {

			if (loading.isDone) {
				// Goto next phase
				state = State.Reveal;
			}

		} else if (state == State.Reveal) {

			currentFade = Mathf.Clamp01(currentFade - Time.deltaTime / duration);
			blackness.color = new Color(0, 0, 0, currentFade);

			if (Mathf.Approximately(currentFade, 0f)) {
				// Remove self
				Destroy(blackness.transform.root.gameObject);
				if (callback != null) callback();
			}
		} else {
			// Only state left is State.Invalid
			Debug.LogError("Level switching created incorrectly. Please use the method SwitchScene.GotoScene(string scene) instead.");
			Destroy(gameObject);
		}
	}

	private static SwitchScene CreateBlackness() {
		var canvas = new GameObject("SceneSwitch fade");
		canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		DontDestroyOnLoad(canvas);

		var go = new GameObject("Blackness");
		go.transform.SetParent(canvas.transform);

		var script = go.AddComponent<SwitchScene>();
		script.blackness = go.AddComponent<Image>();
		script.blackness.color = Color.clear;
		script.state = State.Hide;

		script.blackness.rectTransform.anchorMax = Vector2.one;
		script.blackness.rectTransform.anchorMin =
		script.blackness.rectTransform.sizeDelta =
		script.blackness.rectTransform.anchoredPosition = Vector2.zero;

		return script;
	}

	/// <param name="scene">The scene to be loaded. Make sure it's in build settings!</param>
	/// <param name="callback">Will be runned upon completion, when the scene is loaded and fade has disappeared.</param>
	public static void GotoScene(string scene, float fadeDuration = 0.5f, Action callback = null) {
		var script = CreateBlackness();
		script.duration = fadeDuration;
		script.scene = scene;
		script.callback = callback;
	}

	private enum State {
		/// <summary>Created manually, and will be automatically deleted.</summary>
		Invalid,
		/// <summary>It's getting darker.</summary>
		Hide,
		/// <summary>It's black and the scene is currently loading.</summary>
		Loading,
		/// <summary>It's revealing the scene.</summary>
		Reveal
	}

}
