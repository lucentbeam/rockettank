using UnityEngine;
using System.Collections;

public class EnemyCharacterController : MonoBehaviour {

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
    //public float flightSpeed;
    //public float flightIdleSpeed;
    //public float ascendSpeed;
    //public float descendSpeed;
    //public float height;
    public float reloadPrimarySpeed;
    public float reloadSecondarySpeed;
    public int health;
    public int maxHealth;
    //public int fuel;
    //public int maxFuel;
    public GameObject primaryProjectile;
    public float primaryProjectileSpeed;
    public GameObject secondaryProjectile;
    public float secondaryProjectileSpeed;
    //public GameObject projectileFX;

    // Private variables
    private float speed;
    private float idle;
    //private bool grounded = true;
    private Transform body;
    private Transform projectilePoint;
    private Vector2 currentDirection;
    private Vector2 lastDirection;
    private bool reloadingPrimary = false;
    private bool reloadingSecondary = false;
    private Transform player;


    // Use this for initialization
    void Start () {
        body = this.transform.Find("Body");
        projectilePoint = this.transform.Find("Body/Turret/Barrel/ProjectilePoint");
        //grounded = true;
        speed = groundSpeed;
        idle = groundIdleSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update () {
        CheckHealth();
        //CheckFuel();

        // Check for input
        bool vertical = (Input.GetButton("Vertical") || Input.GetAxis("Vertical") != 0);
        bool horizontal = (Input.GetButton("Horizontal") || Input.GetAxis("Horizontal") != 0);

        /*// Track currentDirection for drifting during flight
        currentDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        currentDirection.Normalize();
        if (vertical || horizontal) {
            lastDirection = currentDirection;
        }*/

        /*// Change between ground or air
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
        }*/

        // Rotate towards objective
        if (horizontal || vertical) {
            Rotate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        /*// Move in a currentDirection
        if (horizontal || vertical) {
            MoveHorizontal(Input.GetAxis("Horizontal") * speed);
            MoveVertical(Input.GetAxis("Vertical") * speed);
        }
        // Drift when flying
        else {
            MoveHorizontal(lastDirection.x * idle);
            MoveVertical(lastDirection.y * idle);
        }*/

        // Firing
        if (!reloadingPrimary) {
            //FirePrimary();
        }
        /*// Firing
        if (Input.GetButtonDown("Fire2") && !reloadingSecondary) {
            FireSecondary();
        }*/
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
        // fast rotation
        float rotSpeed = 360f;

        // distance between target and the actual rotating object
        Vector3 D = player.position - body.position;

        // calculate the Quaternion for the rotation
        Quaternion rot = Quaternion.Slerp(body.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);

        //Apply the rotation 
        body.rotation = rot;

        // put 0 on the axys you do not want for the rotation object to rotate
        body.eulerAngles = new Vector3(0f, body.eulerAngles.y, 0f);
    }

    /*// Take off and Fly
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
    }*/

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
        //GameObject FX = Instantiate(projectileFX, projectilePoint.position, projectilePoint.rotation) as GameObject;

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

    /*// Increase Health
    public void GainLife (int amount) {
        health -= amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }*/

    // End Game
    void CheckHealth () {
        if (health <= 0) {
            // Instantiate FX
            Destroy(gameObject);
        }
    }

    /*******************************************FUEL***************************************/

    /*// Decrease Fuel
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
    }*/
}
