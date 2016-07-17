using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	void Awake () {
		
	}

	void Update()
	{
		checkGameStart ();
		checkGameQuit ();
	}

	public void loadScene(int level)
	{
		SceneManager.LoadScene (level);
	}

	public void runGameover()
	{
		loadScene(2);
	}

	void checkGameStart()
	{
		if ( isTitleScreen() && Input.GetButtonDown("Submit")) {
			loadScene (1);
		}
	}

	void checkGameQuit()
	{
//		if (Input.GetButtonDown ("Cancel")) {
//			if (isTitleScreen ()) {
//				Application.Quit ();
//			} else {
//				loadScene (0);
//			}
//		}
	}
		
	bool isTitleScreen()
	{
		return SceneManager.GetActiveScene ().name == "Title";
	}

	bool isGameoverScreen()
	{
		return SceneManager.GetActiveScene ().name == "Scoreboard";
	}

}
