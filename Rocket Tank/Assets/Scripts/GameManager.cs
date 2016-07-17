using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject playerPrefab;

	public Transform startingPosition;

	public int startingLives = 10;

	// internal variables of state
	private int lives = 3;
	private bool[] playersActive = { false, false };

	private LevelManager levelManager = new LevelManager();

	void Awake () {
		if (instance == null) {
			DontDestroyOnLoad (instance);
			instance = this;
			this.reset();
		} else if (instance != this) {
			Destroy (gameObject);
		}			

	}

	void OnDestroy () {
		if (instance == this) {			
			instance = null;
		}			
	}

	void reset() {
		this.lives = instance.startingLives;
		this.playersActive [0] = false;
		this.playersActive [1] = false;
	}

	// Update is called once per frame
	void Update () {
		checkPlayerJoinRequests();
	}

	void checkPlayerJoinRequests()
	{
		if (!this.playersActive[0] && Input.GetButtonDown ("Join1")) {
			requestNewPlayer ("1");
		}

		if (!this.playersActive[1] && Input.GetButtonDown ("Join2")) {
			requestNewPlayer ("2");
		}
	}

	void requestNewPlayer(string playerNum) {
		if (this.lives > 0) {
			GameObject newPlayer = Instantiate (playerPrefab, startingPosition.position, startingPosition.rotation) as GameObject;
			MainCharacterController playerController = newPlayer.GetComponent<MainCharacterController> ();
			playerController.playerNumber = playerNum;
			this.playersActive [int.Parse (playerNum) - 1] = true;
			this.lives--;
		}
	}

	public void onPlayerDeath(string playerNum) {
		this.playersActive [int.Parse (playerNum) - 1] = false;
		if (hasLost()) {
				this.levelManager.runGameover ();
		}
	}

	public bool hasLost() {
		return MainCharacterController.numberOfPlayers == 0 && this.lives == 0;
	}
}
