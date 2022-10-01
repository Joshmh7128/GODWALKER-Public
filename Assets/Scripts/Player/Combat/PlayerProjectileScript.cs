using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PlayerProjectileScript : MonoBehaviour
{
    [SerializeField] float speed;
    public float damage, localCritMod; // how much damage we deal, the local crit modifier
    
    // vfx
    [SerializeField] GameObject breakParticle, muzzleEffect, normalHitFX, critHitFX, homingFX; // the particle we use on death
    public DamageNumber normalHit, critHit; // normal and critical damage numbers

    RaycastHit hit; // our raycast hit
    [SerializeField] int deathTime = 30;

    [SerializeField] bool usesTrigger;

    // weapon manager instance
    PlayerWeaponManager weaponManager;
    // arena handler instance
    ArenaManager arenaManager;
    // body part manager
    PlayerBodyPartManager bodyPartManager;

    // ability related variables
    public bool isHoming; // does this home to the nearest enemy?
    public bool doesExplode; // does this explode?
    [SerializeField] GameObject playerExplosionPrefab; // the explosion prefab
    Transform homingTarget; // our homing target

    private void Start()
    {
        StartCoroutine(DeathCounter());
        // muzzle flash
        MuzzleFX();
        // get instance
        weaponManager = PlayerWeaponManager.instance;
        arenaManager = ArenaManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;
        // if we are a homing bullet
        if (isHoming) { SetHomingTarget(); Instantiate(homingFX, transform); } 
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
        // if we are homing
        if (isHoming && homingTarget != null)
        {
            // get direction from target to transform
            Vector3 targetDirection = homingTarget.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 5 * Time.deltaTime, -0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    
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
            if (c <= weaponManager.criticalHitChance + localCritMod)
            {   
                // randomly boost damage on critical hits
                damage *= Random.Range(2, 4);
                // spawn critical damage number
                critHit.Spawn(transform.position, damage);
            } else if (c > weaponManager.criticalHitChance + localCritMod)
            {
                // random normal modifier
                damage *= Random.Range(0.9f, 1.25f);
                // spawn normal damage number
                normalHit.Spawn(transform.position, damage);
            }
            enemy.transform.gameObject.GetComponent<EnemyClass>().GetHurt(damage);
            // run our on damage calls
            if (isHoming)
            bodyPartManager.CallParts("OnHomingShotDamage");
        }

    }

    // our custom destruction script
    void Destruction()
    {
        // spawn our death fx
        if (breakParticle != null) Instantiate(breakParticle, transform.position, Quaternion.identity, null);
        // does this bullet explode?
        PlayerExplosionScript explosion = null;
        if (doesExplode)
        {
            explosion = Instantiate(playerExplosionPrefab, transform.position, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
            explosion.damage = damage;
        }
            

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

    // if we are a homing bullet return the closest enemy in that moment and target
    void SetHomingTarget()
    {
        // if we are homing, set homing target to nearest enemy in our active handler's active enemy transform parent 
        Transform localTarget = arenaManager.activeArena.activeParent.GetChild(0); 
        // loop through and find the closest active enemy
        foreach(Transform enemy in arenaManager.activeArena.activeParent)
        {
            // if the distance from this bullet to the enemy is lower than our local target, set that
            if (Vector3.Distance(transform.position, enemy.position) < Vector3.Distance(transform.position, localTarget.position))
                localTarget = enemy;
        }
        
        homingTarget = localTarget;
    }

}
