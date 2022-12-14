using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsSatiation : BodyPartClass
{
    // while moving midair homing shots have a +50% chance to deal critical damage
    PlayerWeaponManager weaponManager; // instance
    bool active, movingMidair;

    public override void OnMoveMidair() => movingMidair = true;
    public override void OnLand() => movingMidair = false;

    public override void PartStart() => weaponManager = PlayerWeaponManager.instance;

    // add critical chance on homing shot
    public override void OnHomingShot()
    {
        active = true;
        if (!active && movingMidair)
            weaponManager.criticalHitModifiers.Add(50f);
    }

    // remove critical chance on normal shot
    public override void OnWeaponFire()
    {
        if (active)
            weaponManager.criticalHitModifiers.Remove(50f);
    }
}
