using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;
    public float damage; // how much damage we deal
    
    // vfx
    [SerializeField] GameObject breakParticle, muzzleEffect, hitFX; // the particle we use on death
    [SerializeField] DamageNumber normalHit, critHit; // normal and critical damage numbers

    RaycastHit hit; // our raycast hit
    [SerializeField] int deathTime = 30;

    [SerializeField] bool usesTrigger;

    // player controller instance
    PlayerWeaponManager weaponManager;

    private void Start()
    {
        StartCoroutine(DeathCounter());
        // muzzle flash
        MuzzleFX();
        // get instance
        weaponManager = PlayerWeaponManager.instance;
        // set the paths to our damage numbers
        normalHit = Resources.Load("VFX/DamageNumbers/Normal-Glow.prefab") as DamageNumber;
        critHit = Resources.Load("VFX/DamageNumbers/Critical-Glow.prefab") as DamageNumber;
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
        if (!usesTrigger)
        Physics.Raycast(transform.position, transform.forward, out hit, 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);   

        // check if we've hit something
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Enemy")
            {
                // check and hit the enemy
                HitEnemy(hit.transform);
                Destruction();
            }

            if (hit.transform.tag != "Enemy")
                // destroy our bullet if we hit anything else
                Destruction();
        }
    }

    void HitEnemy(Transform enemy)
    {
        // if we hit an enemy
        if (enemy.transform.tag == "Enemy")
        {
            // run a chance to see if this is a critical or not
            int c = Random.Range(0, 100);
            // check the chance
            if (c <= weaponManager.criticalHitChance)
            {   
                // randomly boost damage on critical hits
                damage *= Random.Range(2, 4);
                // spawn critical damage number
                critHit.Spawn(transform.position, damage);
                // log that we got criticals
                Debug.Log("Critical hit! Critical chance was: " + weaponManager.criticalHitChance);
            } else if (c > weaponManager.criticalHitChance)
            {
                // random normal modifier
                damage *= Random.Range(0.9f, 1.25f);
                // spawn normal damage number
                critHit.Spawn(transform.position, damage);
            }
            enemy.transform.gameObject.GetComponent<EnemyClass>().GetHurt(damage);
            // our hitfX for hitmarkers
            if (hitFX)
            { Instantiate(hitFX, null); }
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
        yield return new WaitForSecondsRealtime(deathTime);
        Destroy(gameObject);
    }
         
    // muzzle effect
    void MuzzleFX()
    {
        if (muzzleEffect != null)
        // instantiate a muzzle effect at the origin of the shot and parent it there
        Instantiate(muzzleEffect, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin.position, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin.rotation, PlayerWeaponManager.instance.currentWeapon.muzzleOrigin);
    }

    // for trigger based bullets
    private void OnTriggerEnter(Collider other)
    {
        // only do this if the other collider is not a trigger
        if (!other.isTrigger)
        {
            if (other.transform.tag == "Enemy")
            {
                HitEnemy(other.transform);
            }

            if (other.transform.tag != "Player")
            {
                // if we have hit something, destroy ourselves
                Destruction();
            }
        }
    }

}
