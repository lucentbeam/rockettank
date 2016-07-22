using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public GameObject mainCamera;

    private GameObject tank1;
    private GameObject tank2;
	private int playerState = 0;
	private Vector3 cameraOffset;

	void Awake()
	{
		cameraOffset = new Vector3 (0f, 60f, -30f);
	}

    void LateUpdate() {
		if (mainCamera == null) {
			return;
		}
		Vector3 target = new Vector3 ();
		switch(playerState)
		{
		case 1:
			target = tank1.transform.position;
			break;
		case 2:
			target = tank2.transform.position;
			break;
		case 3:
			target = 0.5f * tank1.transform.position + 0.5f * tank2.transform.position;
			break;
		}
		mainCamera.transform.position = target + cameraOffset;
		Vector3 tmp = cameraOffset;
		tmp.y = 10;
		tmp.z = 10;
		mainCamera.transform.LookAt(target+tmp);
    }

	public void removeTank(GameObject tank)
	{
		if (tank1 == tank) {
			tank1 = null;
		} else if (tank2 == tank) {
			tank2 = null;
		} else {
			Debug.Log ("WARNING: non-tank passed to camera controls");
		}
		updateNumberOfPlayers ();
	}

	public void addTank1(GameObject tank)
	{
		tank1 = tank;
		updateNumberOfPlayers ();
	}

	public void addTank2(GameObject tank)
	{
		tank2 = tank;
		updateNumberOfPlayers ();
	}

	void updateNumberOfPlayers()
	{
		// playerState is 0, 1, 2, or 3 depending on if none, player1, player2, or both are active
		playerState = 0;
		playerState += tank1 == null ? 0 : 1;
		playerState += tank2 == null ? 0 : 2;
	}
}