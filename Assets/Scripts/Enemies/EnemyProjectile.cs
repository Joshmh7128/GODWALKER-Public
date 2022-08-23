using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // script exists to manage enemy projectiles and their movemnet in the world

    [SerializeField] enum ProjectileTypes
    {
        kinematic, physics
    }

    [SerializeField] ProjectileTypes projectileType;
    [SerializeField] float speed; // how fast this projectile moves kinematically, or how hard it is launched
    Rigidbody rigidbody;
    [SerializeField] GameObject deathObject; // the object that spawns on death
    [SerializeField] bool facePlayer; // do we face the player
    [SerializeField] int deathTime = 30; // how long to death

    // start runs at the start
    private void Start()
    {
        // setup our rigidbody
        rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(DeathCountdown());

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
    }

    // our physics start
    void StartPhysics()
    {
        // launch our projectile forwards
        rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
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
    }

    // update for kinematics
    void ProcessKinematic()
    {
        // move forward
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    // update for physics
    void ProcessPhysics()
    {
        
    }

    // when we collide with anything
    private void OnTriggerEnter(Collider other)
    {
        // for now, if we collide with anything, die
        if (other == null)
        {
            OnDestroyObject();
        }
    }

    void OnDestroyObject()
    {
        // spawn a deathobject
        Instantiate(deathObject, transform.position, Quaternion.identity, null);
        // then destroy
        Destroy(gameObject);
    }

    IEnumerator DeathCountdown()
    {
        yield return new WaitForSecondsRealtime(deathTime);
        Destroy(gameObject);
    }
}
