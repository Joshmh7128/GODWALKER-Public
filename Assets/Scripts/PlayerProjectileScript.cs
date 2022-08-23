using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject breakParticle; // the particle we use on death
    RaycastHit hit; // our raycast hit
    [SerializeField] int deathTime = 30;

    private void Start()
    {
        StartCoroutine(DeathCounter()); 
    }

    // Update is called once per frame
    void Update()
    {
        // move the bullet
        ProcessMovement();
        // check our raycast
        ProcessHitscan();
    }

    void ProcessMovement()
    {
        // go forward
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }

    void ProcessHitscan()
    {
        // raycast forward
        Physics.Raycast(transform.position, transform.forward, out hit, 1f, Physics.AllLayers);   

        // check if we've hit something
        if (hit.transform != null)
        {
            // if we hit an enemy
            if (hit.transform.tag == "Enemy")
            {
                Debug.Log("Hit enemy");
                hit.transform.gameObject.GetComponent<EnemyClass>().GetHurt();
            }

            // if we have hit something, destroy ourselves
            Destruction();
        }
    }

    // our custom destruction script
    void Destruction()
    {
        // spawn our death fx
        if (breakParticle != null)
        Instantiate(breakParticle, transform.position, Quaternion.identity, null);
        Destroy(gameObject);
    }

    IEnumerator DeathCounter()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
         
}
