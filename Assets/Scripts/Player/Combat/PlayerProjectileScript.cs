using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    public float speed;
    public float damage, localCritMod; // how much damage we deal, the local crit modifier
    public float rageAdd; // how much rage does the act of hitting this shot add to our meter?
    
    // vfx
    [SerializeField] GameObject breakParticle, muzzleEffect, normalHitFX, critHitFX, homingFX, teleportFX; // the particle we use on death

    RaycastHit hit; // our raycast hit
    [SerializeField] float deathTime = 30;

    [SerializeField] bool usesPhysics;
    [SerializeField] float physicsLaunchForce;
    Rigidbody body;

    [SerializeField] bool usesTrigger, doesBounce; // our physics related aspects of this script


    // weapon manager instance
    PlayerWeaponManager weaponManager;
    // arena handler instance
    ArenaManager arenaManager;
    // body part manager
    PlayerBodyPartManager bodyPartManager;
    // player projectile manager instance
    PlayerProjectileManager projectileManager;

    // ability related variables
    public bool startInvBuffer = false;
    public bool isHoming, secondHome; // does this home to the nearest enemy?
    public bool doesExplode; // does this explode?
    public bool doesShockExplode; // does this shock explode?
    public bool isLifesteal; // does this life steal
    public bool isTeleportShot; // is this a shot we can try to teleport to?
    public BodyPartClass teleportCallBack; // when we destroy and are a teleporting shot, send a signal here

    [SerializeField] GameObject playerExplosionPrefab, playerShockExplosionPrefab; // the explosion and shock explosions prefabs
    Transform homingTarget; // our homing target

    private void Start()
    {
        // start death counter
        StartCoroutine(DeathCounter());
        // muzzle flash
        MuzzleFX();
        // get instance
        weaponManager = PlayerWeaponManager.instance;
        arenaManager = ArenaManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;
        projectileManager = PlayerProjectileManager.instance;
        // add ourselves to the projectile manager
        projectileManager.activeProjectileScripts.Add(this);

        // check to ensure we never go over the intended amount of projectiles active in the scene
        SafetyCheck();
        // run our abilities check
        StartCoroutine(ProjectileStartBuffer());

        // if we have a frame start buffer
        if (startInvBuffer) StartCoroutine(InvBuffer());
        // check our raycast before moving
        ProcessHitscan();
    }

    IEnumerator ProjectileStartBuffer()
    {
        TypeCheck();
        yield return new WaitForFixedUpdate();
        StartAbilityCheck();
    }

    void TypeCheck()
    {
        // if we use physics, launch the projectile
        if (usesPhysics)
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * physicsLaunchForce);
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
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 10 * Time.deltaTime, -0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    
    }

    void ProcessHitscan()
    {
        // raycast forward
        if (!usesTrigger && !usesPhysics)
        Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime * 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);   

        if (usesPhysics)
        Physics.Raycast(transform.position, transform.forward, out hit, 1, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        // check if we've hit something
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Enemy")
            {
                // check and hit the enemy
                HitEnemy(hit.transform);
                Destruction(hit.point);
            }

            if (hit.transform.tag != "Enemy" && !doesBounce && hit.transform.tag != "Player")
                // destroy our bullet if we hit anything else
                Destruction(hit.point);
        }
    }

    void HitEnemy(Transform enemy)
    {
        // if we hit an enemy
        if (enemy.transform.tag == "Enemy" && !enemy.transform.gameObject.GetComponent<EnemyClass>().invincible)
        {
            // make sure we have damage
            if (damage == 0)
            {
                damage = weaponManager.currentWeapon.damage;
            }

            // spawn hit fx
            Instantiate(normalHitFX);

            // deal damage to enemy
            enemy.transform.gameObject.GetComponent<EnemyClass>().GetHurt(damage);

            // add our rage
            PlayerRageManager.instance.AddRage(rageAdd);

            // run our on damage calls
            if (isHoming)
            bodyPartManager.CallParts("OnHomingShotDamage");

            if (isLifesteal)
            {
                PlayerStatManager.instance.AddHealth(damage);
            }
        }

        if (enemy.transform.tag == "Enemy" && hit.transform.gameObject.GetComponent<EnemyClass>().invincible)
        {
            // random normal modifier
            damage = 0;
            // spawn normal damage number
        }

    }

    // our custom destruction script
    void Destruction(Vector3 deathPos)
    {
        // make sure we dont have a fixed update buffer running
        if (!startInvBuffer)
        {
            // spawn our death fx
            if (breakParticle != null) Instantiate(breakParticle, deathPos, Quaternion.identity, null);
            
            /// ability related
            // does this bullet explode?
            PlayerExplosionScript explosion = null;
            if (doesExplode)
            {
                explosion = Instantiate(playerExplosionPrefab, deathPos, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
                explosion.damage = damage;
            }

            PlayerShockExplosionScript shock = null;
            if (doesShockExplode)
            {
                shock = Instantiate(playerShockExplosionPrefab, deathPos, Quaternion.identity, null).GetComponent<PlayerShockExplosionScript>();
                shock.damage = damage;
            }
            
            // does this bullet involve teleporting?
            if (isTeleportShot)
            {
                try
                {
                    teleportCallBack.TryTeleport(deathPos);
                } catch { }
            }

            // remove from list
            projectileManager.activeProjectileScripts.Remove(this);            

            Destroy(gameObject); 
        }
    }

    IEnumerator InvBuffer()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        startInvBuffer = false;
    }

    IEnumerator DeathCounter()
    {
        yield return new WaitForSecondsRealtime(deathTime);
        Destruction(new Vector3(999,999,999));
    }

    // the check we run to spawn in our visual fx on this bullet's start
    void StartAbilityCheck()
    {
        // if we are a homing bullet
        if (isHoming) { 
            SetHomingTarget(); Instantiate(homingFX, transform); 
            body = gameObject.GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            usesPhysics = false;
        }
        // if we are a teleporting bullet
        if (isTeleportShot) { Instantiate(teleportFX, transform); }
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
                if (!doesBounce)
                {
                    Destruction(transform.position);
                }

                // if we do bounce, then get the normal and bounce off of it
                if (doesBounce)
                {
                    // raycast forwards
                    RaycastHit hit;
                    Physics.Raycast(transform.position, transform.forward, out hit, 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                    Vector3 newDir = Vector3.Reflect(transform.forward, hit.normal); // reflect
                    transform.rotation = Quaternion.LookRotation(newDir); // rotate
                }
            }
        }
    }

    // if we are a homing bullet return the closest enemy in that moment and target
    void SetHomingTarget()
    {
        // slow down our speed
        speed = speed * 0.5f;

        // if we are homing, set homing target to nearest enemy in our active handler's active enemy transform parent 
        Transform localTarget = arenaManager.activeArena.activeParent.GetChild(0); 
        // loop through and find the closest active enemy
        foreach(Transform enemy in arenaManager.activeArena.activeParent)
        {
            // if the distance from this bullet to the enemy is lower than our local target, set that. also, make sure they are not invincible
            if (Vector3.Distance(transform.position, enemy.position) < Vector3.Distance(transform.position, localTarget.position) && !enemy.GetComponent<EnemyClass>().invincible)
                localTarget = enemy;
        }
        
        homingTarget = localTarget;

        // if we are homing to the 2nd closest target, do the same loop but exclude our localTarget
        Transform secondTarget = arenaManager.activeArena.activeParent.GetChild(0);
        if (secondHome)
        {
            // loop through and find the closest active enemy
            foreach (Transform enemy in arenaManager.activeArena.activeParent)
            {
                // if the distance from this bullet to the enemy is lower than our local target, set that
                if (Vector3.Distance(transform.position, enemy.position) < Vector3.Distance(transform.position, secondTarget.position) && enemy.gameObject != localTarget.gameObject && !enemy.GetComponent<EnemyClass>().invincible)
                    secondTarget = enemy;
            }

            // if (localTarget == secondTarget) Debug.LogError("Homing Projectile has origin Homing target");

            // since this is a 2nd home, look directly at the second target 
            Vector3 direction = secondTarget.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
            homingTarget = secondTarget;

        }
    }

    // safety check
    void SafetyCheck()
    {
      
        // add ourselves to the list if there is not too many
        if (projectileManager.activeProjectileScripts.Count < projectileManager.activeProjectileMax)
        {
            projectileManager.activeProjectileScripts.Add(this);
        } 
        else
        {   // destroy one of the last bullets in the list
            PlayerProjectileScript g = projectileManager.activeProjectileScripts[0];
            // then destroy
            projectileManager.activeProjectileScripts.Remove(g);
            Destroy(g);
        }
    }
}
