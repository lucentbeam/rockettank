using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject playerPrefab;

	public int startingLives = 10;

	// internal variables of state
	private int lives = 3;
	private bool[] playersActive = { false, false };

	private Transform player1StartingPosition;
	private Transform player2StartingPosition;

	// references to sub-managers
	private LevelManager levelManager;
	private CameraManager cameraManager;
	// audio manager

	void Awake () {
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
			this.reset();
		} else if (instance != this) {
			Destroy (gameObject);
		}	
	}

	void Start() {
		levelManager = gameObject.GetComponent<LevelManager> ();
		cameraManager = gameObject.GetComponent<CameraManager> ();
		// get component audio maanger
	}

	void OnLevelWasLoaded(int level) {
		player1StartingPosition = GameObject.Find ("Player1Spawn").transform;
		player2StartingPosition = GameObject.Find ("Player2Spawn").transform;
		cameraManager.mainCamera = GameObject.Find ("Main Camera");
		requestNewPlayer ((int)levelManager.startConditions);
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
		if (levelManager.isSceneLoading) {
			return;
		}
		checkPlayerJoinRequests();
	}

	void checkPlayerJoinRequests()
	{
		if (!this.playersActive[0] && Input.GetButtonDown ("Join1")) {
			requestNewPlayer (1);
		}
		if (!this.playersActive[1] && Input.GetButtonDown ("Join2")) {
			requestNewPlayer (2);
		}
	}

	void requestNewPlayer(int playerNum) {
		if (this.lives > 0) {
			Transform start = playerNum == 1 ? player1StartingPosition : player2StartingPosition;
			if (start == null) { return; }
			GameObject newPlayer = Instantiate (playerPrefab, start.position, start.rotation) as GameObject;
			MainCharacterController playerController = newPlayer.GetComponent<MainCharacterController> ();
			playerController.playerNumber = playerNum.ToString ();
			this.playersActive [playerNum - 1] = true;
			if (playerNum == 1) {
				cameraManager.addTank1 (newPlayer);
			} else {
				cameraManager.addTank2 (newPlayer);
			}
			this.lives--;
		}
	}

	public void onPlayerDeath(GameObject player) {
		this.playersActive [int.Parse (player.GetComponent<MainCharacterController>().playerNumber) - 1] = false;
		cameraManager.removeTank (player);
		if (hasLost()) {
			this.levelManager.runGameover ();
		}
	}

	public bool hasLost() {
		return MainCharacterController.numberOfPlayers == 0 && this.lives == 0;
	}
}
