﻿using UnityEngine;
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
    public GameObject primaryProjectile;
    public float primaryProjectileSpeed;
    public GameObject secondaryProjectile;
    public float secondaryProjectileSpeed;

	public static int numberOfPlayers = 0;

	//public GameObject projectileFX;
	[HideInInspector]public string playerNumber = "1";
	[HideInInspector]public string verticalString = "Vertical";
	[HideInInspector]public string horizontalString = "Horizontal";
	[HideInInspector]public string primaryFire = "Fire";
	[HideInInspector]public string secondaryFire = "AltFire";
	[HideInInspector]public string jump = "Jump";

    // Private variables
    private float speed;
    private float idle;
    private bool grounded = true;
    private Transform body;
    private Rigidbody tankRigidbody;
    private Transform projectilePoint;
    private Vector2 currentDirection;
    private Vector2 lastDirection;
    private bool reloadingPrimary = false;
    private bool reloadingSecondary = false;
    private bool onground = true;
    private float currentHeight;
    private float maxHeight;

    void Awake() {
		numberOfPlayers++;
	}

    // Use this for initialization
    void Start () {
        body = this.transform.Find("Body");
        tankRigidbody = this.GetComponent<Rigidbody>();
        projectilePoint = this.transform.Find("Body/Turret/Barrel/ProjectilePoint");
        grounded = true;
        speed = groundSpeed;
        idle = groundIdleSpeed;
        //Physics.IgnoreLayerCollision(8, 8);
        currentHeight = this.transform.position.y;
        maxHeight = currentHeight + height;
        Debug.Log(currentHeight + " + " + height + " = " + maxHeight);
    }

	void onDestroy() {
		numberOfPlayers--;
	}

	// Update is called once per frame
	void Update () {
        CheckHealth();
        CheckFuel();

        // Check for input
		bool vertical = (Input.GetButton(verticalString+playerNumber) || Input.GetAxis(verticalString+playerNumber) != 0);
		bool horizontal = (Input.GetButton(horizontalString+playerNumber) || Input.GetAxis(horizontalString+playerNumber) != 0);

        // Track currentDirection for drifting during flight
		currentDirection = new Vector2(Input.GetAxis(horizontalString+playerNumber), Input.GetAxis(verticalString+playerNumber));
        currentDirection.Normalize();
        if (vertical || horizontal) {
            lastDirection = currentDirection;
        }

        // Change between ground or air
		if (Input.GetButtonDown(jump+playerNumber)) {
            grounded = !grounded;
            if (grounded) {
                //StopCoroutine("Fly");
                //StartCoroutine("Land");
                this.tankRigidbody.useGravity = true;
            }
            else {
                //StopCoroutine("Land");
                //StartCoroutine("Fly");
                this.tankRigidbody.useGravity = false;
                // Get current height and compare to ceiling, then change height appropriately?
                height = maxHeight - this.transform.position.y;
                Debug.Log(currentHeight + " + " + height + " = " + maxHeight);
                StartCoroutine("Fly");
            }
        }
        /*
        // Rotate towards objective
        if (horizontal || vertical) {
			//Rotate(Input.GetAxis(horizontalString+playerNumber), Input.GetAxis(verticalString+playerNumber));
        }
        
        // Move in a currentDirection
        if (horizontal || vertical) {
			//MoveHorizontal(Input.GetAxis(horizontalString+playerNumber) * speed);
			//MoveVertical(Input.GetAxis(verticalString+playerNumber) * speed);
        }
        // Drift when flying
        else {
            //MoveHorizontal(lastDirection.x * idle);
            //MoveVertical(lastDirection.y * idle);
        }*/

        // Firing
		if (Input.GetButtonDown(primaryFire+playerNumber) && !reloadingPrimary) {
            FirePrimary();
        }
        // Firing
		if (Input.GetButtonDown(secondaryFire+playerNumber) && !reloadingSecondary) {
            FireSecondary();
        }
    }

    void FixedUpdate () {
        // Check for input
        bool vertical = (Input.GetButton(verticalString + playerNumber) || Input.GetAxis(verticalString + playerNumber) != 0);
        bool horizontal = (Input.GetButton(horizontalString + playerNumber) || Input.GetAxis(horizontalString + playerNumber) != 0);

        // Movgin
        if ((horizontal || vertical) ) {
            // If Driving
            if (grounded) {
                // Rotate
                Vector3 rotationVector = new Vector3(Input.GetAxis(horizontalString + playerNumber), 0f, Input.GetAxis(verticalString + playerNumber));
                this.tankRigidbody.MoveRotation(Quaternion.LookRotation(rotationVector, Vector3.up));
                // Move
                this.tankRigidbody.MovePosition(this.transform.position + this.transform.forward * speed * .1f);
            }
            // If Flying
            else {
                // Rotate
                Vector3 rotationVector = new Vector3(Input.GetAxis(horizontalString + playerNumber), 0f, Input.GetAxis(verticalString + playerNumber));
                this.tankRigidbody.MoveRotation(Quaternion.LookRotation(rotationVector, Vector3.up));
                // Move
                Vector3 moveVector = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z);
                this.tankRigidbody.MovePosition(this.transform.position + moveVector * flightSpeed * .1f);
            }
        }
        

    }

    void OnCollisionEnter (Collision c) {
        if (c.gameObject.tag == "Terrain") {
            onground = true;
            Debug.Log("On the ground");
        }
    }

    void OnCollisionExit (Collision c) {
        if (c.gameObject.tag == "Terrain") {
            onground = false;
            Debug.Log("Left the ground");
        }
    }
    /*******************************************MOVEMENT***************************************/
    /*
    // Travel forward and backwards
    void MoveVertical (float currentDirection) {
        //this.transform.Translate(Vector3.forward * Time.deltaTime * currentDirection);
    }

    // Travel left and right
    void MoveHorizontal (float currentDirection) {
        //this.transform.Translate(Vector3.right * Time.deltaTime * currentDirection);
    }

    // Rotate towards currentDirection of movement
    void Rotate (float horizontal, float vertical) {
        
        //Quaternion deltaRotation = Quaternion.Euler(new Vector3(Input.GetAxis(horizontalString+playerNumber), Input.GetAxis(verticalString+playerNumber), 0f));
        //this.tankRigidbody.MoveRotation(this.tankRigidbody.rotation * deltaRotation);
        //body.eulerAngles = new Vector3(this.transform.eulerAngles.x, Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg, this.transform.eulerAngles.z);
    }
    */
    // Take off and Fly
    IEnumerator Fly () {
        for (float h = 0f; h <= height; h += .1f) {
            this.tankRigidbody.MovePosition(this.transform.position + Vector3.up * h);
            Debug.Log(currentHeight + " + " + height + " = " + maxHeight);

            if (this.transform.position.y >= maxHeight) {
                Vector3 maxPosition = new Vector3(this.transform.position.x, maxHeight, transform.position.z);
                this.tankRigidbody.MovePosition(maxPosition);
                break;
            }
            yield return null;
        }
        this.tankRigidbody.velocity = new Vector3(this.tankRigidbody.velocity.x, 0f, this.tankRigidbody.velocity.z);
        Debug.Log(currentHeight + " + " + height + " = " + maxHeight);
    }
    //idle = flightIdleSpeed;


    // Return to ground
    IEnumerator Land () {
        // Lower object to ground
        for (float h = 0f; h <= height; h += .01f) {
            //if (this.transform.position.y > 0f) this.transform.Translate(Vector3.down * descendSpeed * h);
            //if (this.transform.position.y < 0f) this.transform.position = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
            //if (speed > groundSpeed) speed -= h;
            //if (speed < groundSpeed) speed = groundSpeed; 
            yield return null;
        }
        idle = groundIdleSpeed;
    }

    /*******************************************WEAPONS***************************************/

    // Trigger Primary Weapon
	void FirePrimary () {
        // Instantiate Projectile
        GameObject bullet = Instantiate(primaryProjectile, projectilePoint.position, projectilePoint.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().AddForce(projectilePoint.up * primaryProjectileSpeed);

        // Instantiate FX
        //GameObject FX = Instantiate(projectileFX, projectilePoint.position, projectilePoint.rotation) as GameObject;

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
        GameObject bullet = Instantiate(secondaryProjectile, projectilePoint.position, projectilePoint.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().AddForce(projectilePoint.up * secondaryProjectileSpeed);

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
			GameManager.instance.onPlayerDeath(transform.root.gameObject);
			Destroy (transform.root.gameObject);
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
