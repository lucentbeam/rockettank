using UnityEngine;
using System.Collections;

public class MainCharacterController : MonoBehaviour {
    /*  
     *  "this" is "Tank" Parent GameObject that contains children objects of tank meshes.
     *  "body" is the child object with the mesh component
     *  Expected hierarchy:
     *  Tank
     *    -> Body
     *       -> Turret
     *         -> Barrel
     *           -> ProjectilePoint
    */


    // Public variables - editable in Editor
    public float groundSpeed;
    public float groundIdleSpeed;
    public float flightSpeed;
    public float flightIdleSpeed;
    public float ascendSpeed;
    public float descendSpeed;
    public float height;
    public float reloadPrimarySpeed;
    public float reloadSecondarySpeed;
    public int health;
    public int maxHealth;
    public int fuel;
    public int maxFuel;

    // Private variables
    private float speed;
    private float idle;
    private bool grounded = true;
    private Transform body;
    private Vector2 heading;
    private bool reloadingPrimary = false;
    private bool reloadingSecondary = false;

    // Use this for initialization
    void Start () {
        body = this.transform.GetChild(0).transform;
        grounded = true;
        speed = groundSpeed;
        idle = groundIdleSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        CheckHealth();
        CheckFuel();

        // Check for input
		bool vertical = (Input.GetButton("Vertical") || Input.GetAxis("Vertical") != 0);
        bool horizontal = (Input.GetButton("Horizontal") || Input.GetAxis("Horizontal") != 0);

        // Track direction for drifing during flight
        if (vertical || horizontal) {
            Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            direction.Normalize();
            heading = direction;
        }

        // Change between ground or air
        if (Input.GetButtonDown("Jump")) {
            grounded = !grounded;
            if (grounded) {
                StopCoroutine("Fly");
                StartCoroutine("Land");
            }
            else {
                StopCoroutine("Land");
                StartCoroutine("Fly");
            }
        }

        // Rotate towards objective
        if (horizontal || vertical) {
            Rotate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        
        // Move in a direction
        if (horizontal || vertical) {
            MoveHorizontal(Input.GetAxis("Horizontal") * speed);
            MoveVertical(Input.GetAxis("Vertical") * speed);
        }
        // Drift when flying
        else {
            MoveHorizontal(heading.x * idle);
            MoveVertical(heading.y * idle);
        }

        // Firing
        if (Input.GetButtonDown("Fire1") && !reloadingPrimary) {
            FirePrimary();
        }
        // Firing
        if (Input.GetButtonDown("Fire2") && !reloadingSecondary) {
            FireSecondary();
        }
    }

    /*******************************************MOVEMENT***************************************/

    // Travel forward and backwards
    void MoveVertical (float direction) {
        this.transform.Translate(Vector3.forward * Time.deltaTime * direction);
    }

    // Travel left and right
    void MoveHorizontal (float direction) {
        this.transform.Translate(Vector3.right * Time.deltaTime * direction);
    }

    // Rotate towards direction of movement
    void Rotate (float horizontal, float vertical) {
        body.eulerAngles = new Vector3(this.transform.eulerAngles.x, Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg, this.transform.eulerAngles.z);
    }

    // Take off and Fly
    IEnumerator Fly () {
        // Raise object to height
        for (float h = 0f; h <= height; h += .01f) {
            if (this.transform.position.y < height) this.transform.Translate(Vector3.up * ascendSpeed * h);
            if (this.transform.position.y > height) this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
            if (speed < flightSpeed) speed += h;
            if (speed > flightSpeed) speed = flightSpeed;
            yield return null;
            idle = flightIdleSpeed;
        }
    }

    // Return to ground
    IEnumerator Land () {
        // Lower object to ground
        for (float h = 0f; h <= height; h += .01f) {
            if (this.transform.position.y > 0f) this.transform.Translate(Vector3.down * descendSpeed * h);
            if (this.transform.position.y < 0f) this.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
            if (speed > groundSpeed) speed -= h;
            if (speed < groundSpeed) speed = groundSpeed; 
            yield return null;
            idle = groundIdleSpeed;
        }
    }

    /*******************************************WEAPONS***************************************/

    // Trigger Primary Weapon
    void FirePrimary () {
        // Instantiate Projectile
        // Instantiate FX
        StartCoroutine("reloadPrimary");
        reloadingPrimary = true;
    }

    // Delay Primary Weapon From Firing
    IEnumerator reloadPrimary () {
        yield return new WaitForSeconds(reloadPrimarySpeed);
        reloadingPrimary = false;
    }

    // Trigger Secondary Weapon
    void FireSecondary () {
        // Instantiate Projectile
        // Instantiate FX
        StartCoroutine("reloadSecondary");
        reloadingSecondary = true;
    }

    // Delay Secondary Weapon From Firing
    IEnumerator reloadSecondary () {
        yield return new WaitForSeconds(reloadSecondarySpeed);
        reloadingSecondary = false;
    }

    /*******************************************HEALTH***************************************/

    // Decrease Health
    public void LoseLife (int amount) {
        health -= amount;
    }

    // Increase Health
    public void GainLife (int amount) {
        health -= amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    // End Game
    void CheckHealth () {
        if (health <= 0) {
            // Instantiate FX
			GameManager.instance.onPlayerDeath();
        }
    }

    /*******************************************FUEL***************************************/

    // Decrease Fuel
    public void UseFuel (int amount) {
        fuel -= amount;
    }

    // Increase Fuel
    public void GainFuel (int amount) {
        fuel -= amount;
        if (fuel > maxFuel) {
            fuel = maxFuel;
        }
    }

    // Land
    void CheckFuel () {
        if (fuel <= 0) {
            grounded = true;
        }
    }
}
