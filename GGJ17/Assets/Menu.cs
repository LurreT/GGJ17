using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public void StartGame(){
		SceneManager.LoadScene (2);
	}
	public void ExitGame(){
		Application.CancelQuit ();
	}
}
