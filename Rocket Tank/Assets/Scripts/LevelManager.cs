using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum StartingConditions {
	BOTH_PLAYERS,
	PLAYER_ONE,
	PLAYER_TWO
}

public class LevelManager : MonoBehaviour {

	[HideInInspector]public bool isSceneLoading;
	[HideInInspector]public StartingConditions startConditions;

	void Awake () {
		isSceneLoading = false;
	}

	void Update()
	{
		checkGameStart ();
		checkGameQuit ();
	}

	void OnLevelWasLoaded(int level)
	{
		isSceneLoading = false;
	}

	public void loadScene(string level)
	{
		SceneManager.LoadScene (level);
	}

	public void runGameover()
	{
		loadScene("Scoreboard");
	}

	void checkGameStart()
	{
		if ( isTitleScreen() && (Input.GetButtonDown("Join1") || Input.GetButtonDown("Join2"))) {
			isSceneLoading = true;
			startConditions = Input.GetButton("Join1") ? StartingConditions.PLAYER_ONE : StartingConditions.PLAYER_TWO;
			loadScene ("BenDevScene");
		}
	}

	void runGameQuit() {
		Application.Quit ();
	}

	void checkGameQuit()
	{
		if (isGameoverScreen() && (Input.GetButtonDown("Join1") || Input.GetButtonDown("Join2"))) {
			loadScene ("Title");
		}
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
