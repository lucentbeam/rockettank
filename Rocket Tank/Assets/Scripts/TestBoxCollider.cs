using UnityEngine;
using System.Collections;

public class TestBoxCollider : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		other.transform.root.GetComponent<MainCharacterController> ().LoseLife (5000000);
	}
}
