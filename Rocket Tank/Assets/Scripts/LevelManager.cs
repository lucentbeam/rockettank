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

	public void runGameOver()
	{
		loadScene(SceneManager.GetSceneByName ("Scoreboard").buildIndex);
	}

	void checkGameStart()
	{
		if ( isTitleScreen() && Input.GetButtonDown("Submit")) {
			loadScene (1);
		}
	}

	void checkGameQuit()
	{
		if (Input.GetButtonDown ("Cancel")) {
			if (isTitleScreen ()) {
				Application.Quit ();
			} else {
				loadScene (0);
			}
		}
	}
		
	// game state shortcuts(?)
	bool isTitleScreen()
	{
		return SceneManager.GetActiveScene ().name == "Title";
	}

	bool isGameoverScreen()
	{
		return SceneManager.GetActiveScene ().name == "Scoreboard";
	}

}
