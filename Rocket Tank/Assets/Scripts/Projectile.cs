using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    // Initial variables
    public int damage;

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(9, 9);
        Physics.IgnoreLayerCollision(10, 11);
        Physics.IgnoreLayerCollision(11, 11);
        Physics.IgnoreLayerCollision(9, 11);
    }
    
    //void OnCollisionEnter(Collision collision) {
    void OnTriggerEnter(Collider collision) {
        GameObject collidedObject = collision.gameObject;
        // Hit a Player
        if (collidedObject.tag == "Player" || collidedObject.tag == "Player2") {
            Debug.Log("Hit a Player");
            Destroy(gameObject);
            collidedObject.transform.root.GetComponent<MainCharacterController>().LoseLife(damage);
        }
        // Hit an Enemy
        else if (collidedObject.tag == "Enemy") {
            Debug.Log("Hit an Enemy");
            Destroy(gameObject);
            collidedObject.transform.root.GetComponent<EnemyCharacterController>().LoseLife(damage);
        }
        // Hit a Boundary
        else if (collidedObject.tag == "Boundary") {
            Debug.Log("Hit a Boundary");
            Destroy(gameObject);
        }
        // Hit the Terrain
        else if (collidedObject.tag == "Terrain") {
            Debug.Log("Hit the Terrain");
            Destroy(gameObject);
        }
        // Hit another Projectile
        else if (collidedObject.tag == "Projectile") {
            Debug.Log("Hit a Projectile");
            Destroy(gameObject);
        }
        // Hit something else
        else {
            Debug.Log(collidedObject.tag);
            Destroy(gameObject);
        }
    }
    
}
