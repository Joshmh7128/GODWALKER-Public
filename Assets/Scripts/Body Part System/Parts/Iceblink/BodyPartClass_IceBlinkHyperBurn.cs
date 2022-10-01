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
            // instantiate a new projectile at the explosion point
            PlayerProjectileScript activeProjectile = Instantiate(projectile, explosion.transform.position, Quaternion.identity).GetComponent<PlayerProjectileScript>();
            activeProjectile.isHoming = true;
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
