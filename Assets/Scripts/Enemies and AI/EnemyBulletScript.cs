using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    // variables
    public Transform bulletTarget; // what is the target of our bullet?
    [SerializeField] float bulletSpeed; // what is the speed of our bullet?
    [SerializeField] GameObject cubePuff; // our break particle effect
    [SerializeField] ParticleSystem ourParticleSystem; // our particle effect
    Transform enemyManager;
    Transform playerTransform;
    [SerializeField] bool speedsUp, targetPlayer; // does our bullet linearly speed up?
    public Vector3 customDirection; // leave blank if no direction

    // for when our bullet is instantiated
    private void Start()
    {
        // find player
        if (playerTransform == null)
        {
            playerTransform = UpgradeSingleton.Instance.player.transform;
        }

        transform.parent = null;

        // turn bullet
        if (bulletTarget == null && targetPlayer == true)
        {
            bulletTarget = playerTransform;
            transform.LookAt(bulletTarget); 
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
    }

    // Update is called once per frame
    void Update()
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

    IEnumerator SafetyKill()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Debug.Log("Bullet Collision");

        // destroy if it hits the environment
        if (collision.CompareTag("Environment"))
        {
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }

        // if this hits the player
        if (collision.CompareTag("Player"))
        {
            playerTransform.gameObject.GetComponent<PlayerController>().AddHP(-1);
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }

        // if this hits a breakable
        if (collision.CompareTag("Breakable"))
        {
            // anything with the Breakable tag will be a chunk and have a BreakableBreak function
            collision.GetComponent<BreakableChunk>().BreakableBreak();
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }
}
