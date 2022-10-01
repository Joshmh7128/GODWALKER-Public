using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_IceBlinkHyperBurn : BodyPartClass
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
                explosion.used = true;
            }
        }
    }



    // local function to request that the next shot be a homing shot
    void RequestHoming()
    {
        weaponManager.currentWeapon.requestHomingShot = true;
    }

    // local function used to request that the next shot be an explosion
    void RequestExplosion()
    {
        weaponManager.currentWeapon.requestExplodingShot = true;
    }
}
