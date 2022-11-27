using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // script exists to manage enemy projectiles and their movemnet in the world

    public enum ProjectileTypes
    {
        kinematic, physics, curves, homing, ring
    }

    public ProjectileTypes projectileType;
    [SerializeField] float speed; // how fast this projectile moves kinematically, or how hard it is launched
    Rigidbody localRigidbody;
    [SerializeField] GameObject deathObject; // the object that spawns on death
    [SerializeField] bool facePlayer; // do we face the player
    [SerializeField] bool invincible; // are we invincible?
    [SerializeField] int deathTime; // how long to death
    [SerializeField] float openLifetime = 6f;
    public float damage; // how much damage does this deal?

    // our curve speed
    [SerializeField] float homingSpeed; // how fast we home towards the player
    [SerializeField] float curveSpeed; // how fast we curve in any direction
    Vector3 curveVector; // our curve vector
    [SerializeField] float ringExpandSpeed; // how fast the rings expand from this enemy

    // get our stat manager
    PlayerStatManager statManager;


    // start runs at the start
    private void Start()
    {
        // setup our rigidbody
        localRigidbody = GetComponent<Rigidbody>();

        StartCoroutine(DeathCountdown());

        // setup the stat manager
        statManager = PlayerStatManager.instance;

        // if we face the player
        if (facePlayer)
        {
            transform.LookAt(PlayerController.instance.transform.position);
        }

        if (projectileType == ProjectileTypes.kinematic)
        {

        }

        if (projectileType == ProjectileTypes.physics)
        {
            StartPhysics();
        }

        if (projectileType == ProjectileTypes.curves)
        {
            // calculate our curve vector
            curveVector = new Vector3(Random.Range(-curveSpeed, curveSpeed), Random.Range(-curveSpeed, curveSpeed), Random.Range(-curveSpeed, curveSpeed));
        }

        if (projectileType == ProjectileTypes.homing)
        {
            // home in on the player
            // if we are homing, get direction from target to transform
            Vector3 targetDirection = PlayerController.instance.transform.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 10 * Time.deltaTime, -0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        if (projectileType == ProjectileTypes.ring)
        {
            // expand
             // transform.localScale += new Vector3(ringExpandSpeed, 0, ringExpandSpeed);
        }
    }

    // our physics start
    void StartPhysics()
    {
        // launch our projectile forwards
        localRigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    void StartKinematic()
    {
        // shoot the bullet

    }

    private void Update()
    {
        if (projectileType == ProjectileTypes.kinematic)
        { 
            ProcessKinematic();
        }

        if (projectileType == ProjectileTypes.physics)
        {
            ProcessPhysics();
        }

        if (projectileType == ProjectileTypes.curves)
        {
            ProcessKinematic();
            ProcessCurves();
        }

        if (projectileType == ProjectileTypes.homing)
        {
            ProcessKinematic();
            ProcessHoming();
        }

        if (projectileType == ProjectileTypes.ring)
        {
            // expand
            transform.localScale += new Vector3(ringExpandSpeed * Time.deltaTime, 0, ringExpandSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (openLifetime > 0)
            openLifetime--;
    }

    // update for kinematics
    void ProcessKinematic()
    {
        // move forward
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    // update for curving shots
    void ProcessCurves()
    {
        // rotate the bullet slightley at a random vector multiplied by our rotation delta
        transform.eulerAngles += curveVector;
    }

    void ProcessHoming()
    {
        // home in on the player
        // if we are homing, get direction from target to transform
        Vector3 targetDirection = PlayerController.instance.transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 10 * Time.deltaTime, -0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // once we get close enough to the player, stop homing
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < 6)
        {
            projectileType = ProjectileTypes.kinematic;
        }

    }

    // update for physics
    void ProcessPhysics()
    {
        
    }

    // when we collide with anything
    private void OnTriggerEnter(Collider other)
    {
        // for now, if we collide with anything, die
        if (other.transform != null && (openLifetime <= 0))
        {
            // what did we hit?
            if (other.transform.tag == "Player")
            {
                // trigger a hurt on the stat manager
                statManager.TakeDamage(damage); // oof ouch yikes
            }

            // always destroy
            if (!invincible && other.transform.tag != "Enemy")
            OnDestroyObject();
        }
    }

    // for while we are colliding inside of things, since we may be shot from inside of an object
    private void OnTriggerStay(Collider other)
    {
        // for now, if we collide with anything, die
        if (other.transform != null && (openLifetime <= 0))
        {
            // what did we hit?
            if (other.transform.tag == "Player")
            {
                // trigger a hurt on the stat manager
                statManager.TakeDamage(damage); // oof ouch yikes
            }

            // always destroy
            if (!invincible && other.transform.tag != "Enemy")
            OnDestroyObject();
        }
    }

    void OnDestroyObject()
    {
        // spawn a deathobject
        if (deathObject)
        Instantiate(deathObject, transform.position, Quaternion.identity, null);
        // then destroy
        Destroy(gameObject);
    }

    IEnumerator DeathCountdown()
    {
        yield return new WaitForSecondsRealtime(deathTime);
        OnDestroyObject();
    }
}