using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    // variables
    public Transform bulletTarget; // what is the target of our bullet?
    [SerializeField] float bulletSpeed, homingTime, homingDistance, damage; // what is the speed of our bullet? how long does it home?
    [SerializeField] GameObject cubePuff; // our break particle effect
    [SerializeField] ParticleSystem ourParticleSystem; // our particle effect
    Transform enemyManager;
    Transform playerTransform;
    [SerializeField] bool speedsUp, targetPlayer, usesPhysics, overrideRaycast, doesBounce, usesParent = false; // does our bullet linearly speed up?
    public Vector3 customDirection; // leave blank if no direction

    // for when our bullet is instantiated
    private void Start()
    {
        // find player
        if (playerTransform == null)
        {
            playerTransform = UpgradeSingleton.Instance.player.transform;
        }

        if (!usesParent)
        {
            transform.parent = null;
        }


        // turn bullet
        if (bulletTarget == null && targetPlayer == true)
        {
            bulletTarget = playerTransform;
            transform.LookAt(bulletTarget); 
        }

        if (usesPhysics && gameObject.GetComponent<Rigidbody>() != null)
        {
            // if this uses physics, launch it forwards
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
        }

        // start the safety kill
        StartCoroutine("SafetyKill");
    }

    private void FixedUpdate()
    {
        if (speedsUp)
        {
            bulletSpeed++;
            bulletSpeed++;
        }

        homingTime--;

        if (Vector3.Distance(playerTransform.position, transform.position) > homingDistance)
        {
            homingTime = 0f;
        }

        if (homingTime > 0)
        {
            transform.LookAt(playerTransform);
        }

        if (!usesPhysics)
        {
            // move bullet
            if (customDirection != new Vector3(0, 0, 0))
            {
                transform.Translate(customDirection * bulletSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
            }
        }


        // perform a raycast in our forward direction to see if we should break, if we dont use physics
        if (!usesPhysics && !overrideRaycast)
        {
            DoubleCheckRaycast();
        }

    }

    void DoubleCheckRaycast()
    {
        RaycastHit hit;
        // raycast
        Physics.Raycast(transform.position, transform.forward, out hit, bulletSpeed * 0.1f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position, transform.forward * bulletSpeed * 0.1f, Color.red);

        if (hit.transform != null)
        {
            if (hit.transform.tag != "Player")
            {
                DestroyBullet();
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator SafetyKill()
    {   
        yield return new WaitForSeconds(10f);
        if (!usesParent)
            DestroyBullet();
    }

    public void DestroyBullet()
    {
        if (cubePuff != null)
        {
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Debug.Log("Bullet Collision");

        // destroy if it hits the environment
        if (collision.transform.tag == "Environment")
        {
            if (overrideRaycast && !doesBounce)
            {
                Destroy(gameObject);
            }

            if (doesBounce)
            {
                // forward direction
                Vector3 forward = transform.forward;
                // do a raycast directly forward
                RaycastHit hit;
                Physics.Raycast(transform.position, forward, out hit, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Collide);
                // 
            }
        }// if this hits the player
        else if (collision.transform.tag == "Player")
        {
            // hurt the player
            playerTransform.gameObject.GetComponent<PlayerController>().AddHP(damage);
            if (!usesParent)
            DestroyBullet();
        }
        else if (collision.transform.tag == "Enemy")
        {
            // do nothing (here because we did previously want to do something, and will)
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!usesPhysics)
        DestroyBullet();
    }

}
