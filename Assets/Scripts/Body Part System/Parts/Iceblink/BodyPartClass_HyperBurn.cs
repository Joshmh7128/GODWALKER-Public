using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_HyperBurn : BodyPartClass
{
    // All shots are now homing, lol
    /// <summary>
    /// - All bullets now Explode
    /// - Whenever an Explosion deals damage to multiple targets, your next shot is a Double Shot
    /// - All Double Shots and Homing Shots Explode
    /// - Whenever you are damaged by an Explosion, fire 20 Life-steal Homing Shots
    /// </summary>

    PlayerWeaponManager weaponManager; // our weapon manager

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    public override void OnWeaponFire()
    {
        RequestHoming();
    }

    public override void OnDoubleShot()
    {
        RequestHoming();
    }

    // local function to request that the next shot be a homing shot
    void RequestHoming()
    {
        weaponManager.currentWeapon.requestHomingShot = true;
    }
}
