using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    private GameObject tank1;
    private GameObject tank2;

	// Use this for initialization
	void Start () {
        tank1 = GameObject.FindGameObjectWithTag("Player");
        tank2 = GameObject.FindGameObjectWithTag("Player2");
    }
	
    void LateUpdate() {
        this.transform.position = tank1.transform.position + new Vector3(0f, 37f, -10f);
    }
}
