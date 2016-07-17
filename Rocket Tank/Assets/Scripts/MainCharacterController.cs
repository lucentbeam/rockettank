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
    private Transform projectilePoint;
    private Vector2 currentDirection;
    private Vector2 lastDirection;
    private bool reloadingPrimary = false;
    private bool reloadingSecondary = false;

	void Awake() {
		numberOfPlayers++;
	}

    // Use this for initialization
    void Start () {
        body = this.transform.Find("Body");
        projectilePoint = this.transform.Find("Body/Turret/Barrel/ProjectilePoint");
        grounded = true;
        speed = groundSpeed;
        idle = groundIdleSpeed;
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
			Rotate(Input.GetAxis(horizontalString+playerNumber), Input.GetAxis(verticalString+playerNumber));
        }
        
        // Move in a currentDirection
        if (horizontal || vertical) {
			MoveHorizontal(Input.GetAxis(horizontalString+playerNumber) * speed);
			MoveVertical(Input.GetAxis(verticalString+playerNumber) * speed);
        }
        // Drift when flying
        else {
            MoveHorizontal(lastDirection.x * idle);
            MoveVertical(lastDirection.y * idle);
        }

        // Firing
		if (Input.GetButtonDown(primaryFire+playerNumber) && !reloadingPrimary) {
            FirePrimary();
        }
        // Firing
		if (Input.GetButtonDown(secondaryFire+playerNumber) && !reloadingSecondary) {
            FireSecondary();
        }
    }

    /*******************************************MOVEMENT***************************************/

    // Travel forward and backwards
    void MoveVertical (float currentDirection) {
        this.transform.Translate(Vector3.forward * Time.deltaTime * currentDirection);
    }

    // Travel left and right
    void MoveHorizontal (float currentDirection) {
        this.transform.Translate(Vector3.right * Time.deltaTime * currentDirection);
    }

    // Rotate towards currentDirection of movement
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
        }
        idle = flightIdleSpeed;
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
			GameManager.instance.onPlayerDeath(playerNumber);
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
