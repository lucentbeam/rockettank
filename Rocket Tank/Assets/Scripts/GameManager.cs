using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Transform playerPrefab;

	public Transform startingPosition;

	public int startingLives = 10;

	// internal variables of state
	private int lives = 3;

	private LevelManager levelManager = new LevelManager();

	void Wake () {
		if (instance == null) {
			DontDestroyOnLoad (instance);
			instance = this;
			this.reset();
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	void reset() {
		this.lives = instance.startingLives;
	}

	// Update is called once per frame
	void Update () {
		// TODO: replace this with some care to check for joystick number
		if (Input.GetKeyDown ("space")) {
			if (this.lives > 0) {
				requestNewPlayer ();
				this.lives--;
			} else {
				Debug.Log (this.levelManager);
				this.levelManager.runGameover ();
			}
		}
	}

	void requestNewPlayer() {
		Instantiate (playerPrefab, startingPosition.position, startingPosition.rotation);
	}

	public void onPlayerDeath() {
		if (this.lives == 0) {
			if (GameObject.FindGameObjectsWithTag ("Player").Length == 0) {
				this.levelManager.runGameover ();
			}
		}
	}
}
