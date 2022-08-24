using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;
    public float damage; // how much damage we deal
    [SerializeField] GameObject breakParticle, muzzleEffect, hitFX; // the particle we use on death
    RaycastHit hit; // our raycast hit
    [SerializeField] int deathTime = 30;

    private void Start()
    {
        StartCoroutine(DeathCounter());
        // muzzle flash
        MuzzleFX();
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
        Physics.Raycast(transform.position, transform.forward, out hit, 2f, Physics.AllLayers);   

        // check if we've hit something
        if (hit.transform != null)
        {
            // if we hit an enemy
            if (hit.transform.tag == "Enemy")
            {
                hit.transform.gameObject.GetComponent<EnemyClass>().GetHurt(damage);
                // our hitfX for hitmarkers
                if (hitFX)
                { Instantiate(hitFX, null); }
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
        // our break visual fx
        Instantiate(breakParticle, transform.position, Quaternion.identity, null);
        
        Destroy(gameObject);
    }

    IEnumerator DeathCounter()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
         
    // muzzle effect
    void MuzzleFX()
    {
        // instantiate a muzzle effect at the origin of the shot and parent it there
        Instantiate(muzzleEffect, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin.position, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin.rotation, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin);
    }

}
