using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_IceblinkHyperBurn : BodyPartClass
{
    // All shots are now homing, lol
    /// <summary>
    /// - All bullets now Explode
    /// - Whenever an Explosion deals damage to multiple targets, your next shot is a Double Shot
    /// - All Double Shots and Homing Shots Explode
    /// - Whenever you are damaged by an Explosion, fire 20 Life-steal Homing Shots
    /// </summary>

    PlayerWeaponManager weaponManager; // our weapon manager
    PlayerProjectileManager projectileManager; // our projectile manager

    // our part start
    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
        projectileManager = PlayerProjectileManager.instance;
    }

    public override void OnBodyPartPickup()
    {
        RequestExplosion();
    }

    // whenever the weapon is fired
    public override void OnWeaponFire()
    {
        RequestExplosion();
    }

    // whenever a double shot is fired
    public override void OnDoubleShot()
    {
        RequestExplosion();
    }

    // whenever an explosion deals damage to an enemy a Homing shot is fired out of the explosion
    public override void OnExplosionDamage()
    {
        // store our current weapon's bullet as a prefab
        GameObject projectile = weaponManager.currentWeapon.bulletPrefab;

        // loop through out list of explosions 
        foreach (PlayerExplosionScript explosion in projectileManager.explosionScripts)
        {
            if (explosion.enemiesHit > 0 && !explosion.used)
            {
                // instantiate a new projectile at the explosion point
                PlayerProjectileScript activeProjectile = Instantiate(projectile, explosion.transform.position, Quaternion.LookRotation(Vector3.up)).GetComponent<PlayerProjectileScript>();
                activeProjectile.isHoming = true;
                activeProjectile.secondHome = true; // we want this bullet to go to the 2nd closest target from its origin, not the closest
                activeProjectile.doesExplode = true;
                activeProjectile.startInvBuffer = true; // this bullet needs to exist for one fixedupdate before destroying
                activeProjectile.damage = weaponManager.currentWeapon.damage * 0.1f;
                explosion.used = true;
            }
        }
    }
    
    // whenever multiple damage happens, request a double shot
    public override void OnMultipleExplosionDamage()
    {
        RequestDoubleShot();
    }

    // whenever an explosion damages the player
    public override void OnExplosionDamagePlayer()
    {        
        // store our current weapon's bullet as a prefab
        GameObject projectile = weaponManager.currentWeapon.bulletPrefab;

        // fire 20 lifestealing shots
        for (int i = 0; i < 2; i++)
        {
            // instantiate a new projectile at the explosion point
            PlayerProjectileScript activeProjectile = Instantiate(projectile, PlayerController.instance.transform.position, Quaternion.LookRotation(Vector3.up)).GetComponent<PlayerProjectileScript>();
            activeProjectile.isHoming = true;
            activeProjectile.isLifesteal = true;
            activeProjectile.startInvBuffer = true; // this bullet needs to exist for one fixedupdate before destroying
        }
    }

    void RequestDoubleShot()
    {
        weaponManager.currentWeapon.requestDoubleShot = true;
    }

    

    // local function used to request that the next shot be an explosion
    void RequestExplosion()
    {
        weaponManager.currentWeapon.requestExplodingShot = true;
    }
}
