using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PlayerProjectileScript : MonoBehaviour
{
    public float speed;
    public float damage, localCritMod; // how much damage we deal, the local crit modifier
    public float rageAdd; // how much rage does the act of hitting this shot add to our meter?
    public float kickDistancePower, kickMovementPower; // our distance and kick power
    public Vector3 kickOriginPoint; // our kick origin point
    public WeaponClass.KickOrigins kickOrigin; // our kick origin selection

    // vfx
    [SerializeField] GameObject breakParticle, muzzleEffect, normalHitFX, critHitFX, homingFX, teleportFX; // the particle we use on death
    [SerializeField] DamageNumber rageNumber;

    RaycastHit hit; // our raycast hit
    public float deathTime = 30;
    [SerializeField] float homingHangTime; // how long do we wait in seconds before homing?

    [SerializeField] bool usesPhysics;
    [SerializeField] Vector3 adjustFire;
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
    [Header("Abilities")]
    public bool isHoming;
    public bool secondHome; // does this home to the nearest enemy?
    public bool doesExplode; // does this explode?
    public bool doesShockExplode; // does this shock explode?
    public bool doesSlag; // does this projectile apply slag to enemies?
    public bool isLifesteal; // does this life steal
    public bool isTeleportShot; // is this a shot we can try to teleport to?
    public bool appliesKick; // does this apply kick to our enemies?
    [Header("Conspecifics")]
    [SerializeField] bool invincible; // is this shot invincible?
    public BodyPartClass teleportCallBack; // when we destroy and are a teleporting shot, send a signal here

    [SerializeField] GameObject playerExplosionPrefab, playerShockExplosionPrefab; // the explosion and shock explosions prefabs
    Transform homingTarget; // our homing target
    [SerializeField] bool unparent;

    [SerializeField] PlayerProjectileScript copyProjectile; // which projectile should we work off of work rage meter?

    private void Start()
    {
        if (unparent)
            transform.parent = null;

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

        // manually set damage
        if (!PlayerRageManager.instance.godmoding)
            damage = weaponManager.currentWeapon.damage;

        // if (PlayerRageManager.instance.godwalking)
        damage = weaponManager.currentWeapon.damage * PlayerRageManager.instance.damageMult[(int)PlayerRageManager.instance.rageLevel];


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
        // adjust our direction on the firing of our object
        if (adjustFire != Vector3.zero)
            transform.eulerAngles += adjustFire;
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
        // raycast where we're going and where we've gone
        if (!usesTrigger && !usesPhysics)
        Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime * 4, Physics.AllLayers, QueryTriggerInteraction.Ignore);   
        // Physics.Raycast(transform.position, -transform.forward, out hit, speed * Time.deltaTime, Physics.AllLayers, QueryTriggerInteraction.Ignore);   

        if (usesPhysics)
        Physics.Raycast(transform.position, transform.forward, out hit, 1, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        // check if we've hit something
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Enemy")
            {
                // check and hit the enemy
                HitEnemy(hit.transform, hit.point);
                // if we're not an invincible bullet, destroy
                if (!invincible)
                    Destruction(hit.point);
            }

            if (hit.transform.tag != "Enemy" && !doesBounce && hit.transform.tag != "Player")
                // destroy our bullet if we hit anything else
                if (!invincible)
                    Destruction(hit.point);
        }
    }

    void HitEnemy(Transform enemy, Vector3 hitpoint)
    {
        // if we have a copy parent, copy our rage
        if (copyProjectile) rageAdd = copyProjectile.rageAdd;

        EnemyClass eclass = enemy.transform.gameObject.GetComponent<EnemyClass>();

        float displayDamage = 0;
        Color showColor = Color.white;

        // if we hit an enemy
        if (enemy.transform.tag == "Enemy" && !eclass.invincible)
        {

            // set display damage
            displayDamage = damage;

            // spawn hit fx
            Instantiate(normalHitFX);

            // deal damage to enemy
            eclass.GetHurt(damage, EnemyClass.ElementalProtection.none);

            if (eclass.energyShieldHP <= 0 && eclass.explosiveArmorHP <= 0)
            {
                PlayerRageManager.instance.AddRage(rageAdd * eclass.rageModifier);
            }

            // check the status of the enemy's shields
            if (eclass.energyShieldHP > 0 && doesShockExplode)
            {
                // if we shock and deal shocking damage, give rage
                PlayerRageManager.instance.AddRage(rageAdd * eclass.rageModifier * 1.25f);
                showColor = Color.red;
                displayDamage *= 1.25f;
            } 
            else if (eclass.energyShieldHP > 0 && !doesShockExplode)
            {
                // if we shock and deal shocking damage, give rage
                PlayerRageManager.instance.AddRage(rageAdd * eclass.rageModifier * 0.3f);
                showColor = Color.yellow;
                displayDamage *= 0.3f;
            }

            if (eclass.explosiveArmorHP > 0 && doesExplode)
            {
                // if we explode and deal exploding damage, give rage
                PlayerRageManager.instance.AddRage(rageAdd * eclass.rageModifier * 1.25f);
                showColor = Color.red;
                displayDamage *= 1.25f;
            }
            else if (eclass.explosiveArmorHP > 0 && !doesExplode)
            {
                // if we shock and deal shocking damage, give rage
                PlayerRageManager.instance.AddRage(rageAdd * eclass.rageModifier * 0.3f);
                showColor = Color.yellow;
                displayDamage *= 0.3f;
            }


            // apply our elemental effects
            // do we slag them?
            if (doesSlag)
            {
                eclass.ApplyEffect(EnemyClass.Effects.Slag);
            }

            if (isLifesteal)
            {
                PlayerStatManager.instance.AddHealth(damage);
            }

            // show how much damage we deal
            rageNumber.Spawn(hitpoint, displayDamage, showColor);

            // do we update our enemy knockback?
            if (kickOrigin == WeaponClass.KickOrigins.bullet)
                kickOriginPoint = transform.position;

            // then apply our knockback, only kick if we can
            if (kickOrigin != WeaponClass.KickOrigins.none)
                eclass.ApplyKickMovement(kickDistancePower, kickMovementPower, kickOriginPoint);

        }

        if (enemy.transform.tag == "Enemy" && eclass.invincible)
        {
            damage = 0;
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
                explosion.rageAdd = rageAdd/3;
            }

            PlayerShockExplosionScript shock = null;
            if (doesShockExplode)
            {
                shock = Instantiate(playerShockExplosionPrefab, deathPos, Quaternion.identity, null).GetComponent<PlayerShockExplosionScript>();
                shock.damage = damage;
                shock.rageAdd = rageAdd/3;
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
            try
            {
                projectileManager.activeProjectileScripts.Remove(this);
            } catch {} 

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
            StartCoroutine(HomingBuffer()); Instantiate(homingFX, transform);
            try
            {
                body = gameObject.GetComponent<Rigidbody>();
                body.isKinematic = true;
                body.useGravity = false;
                usesPhysics = false;
            } catch { }
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
                HitEnemy(other.transform, transform.position);
            }

            if (other.transform.tag != "Player")
            {
                // if we have hit something, destroy ourselves
                if (!doesBounce)
                {
                    if (!invincible)
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

    IEnumerator HomingBuffer()
    {
        yield return new WaitForSecondsRealtime(homingHangTime);
        SetHomingTarget();
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * speed * Time.fixedDeltaTime * 2);
    }
}
