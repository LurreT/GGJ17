﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public GameObject pauseMenu;
	[SceneDropDown]
	public string sceneOnStart;

	public void StartGame(){
		SwitchScene.GotoScene(sceneOnStart);
	}
	public void ExitGame(){
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit ();
#endif
	}
	public void Pause(){
		Time.timeScale = 0;
		pauseMenu.SetActive (true);
	}
	public void UnPause(){
		Time.timeScale = 1;
		pauseMenu.SetActive (false);
	}
}
