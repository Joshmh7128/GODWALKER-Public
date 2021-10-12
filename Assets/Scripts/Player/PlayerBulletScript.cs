using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
    // variables
    public Transform bulletTarget; // what is the target of our bullet?
    [SerializeField] float bulletSpeed; // what is the speed of our bullet?
    [SerializeField] GameObject cubePuff; // our particle effect
    public enum bulletTypes
    {
        Projectile,
        Hitscan
    }

    public bulletTypes bulletType;

    // for when our bullet is instantiated
    private void Start()
    {
        // turn bullet
        transform.LookAt(bulletTarget);
   
        // start the safety kill
        StartCoroutine("SafetyKill");
    }

    // Update is called once per frame
    void Update()
    {
        // move bullet
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    IEnumerator SafetyKill()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // destroy if it hits the environment
        if (collision.CompareTag("Environment"))
        {
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

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyClass>().TakeDamage(1);
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }
}
