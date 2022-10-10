using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsSatiation : BodyPartClass
{
    // while moving down homing shots have a +50% chance to deal critical damage
    PlayerWeaponManager weaponManager; // instance
    bool satActive, movingDown;

    public override void OnMoveDown() => movingDown = true;
    public override void OnMoveUp() => movingDown = false;
    public override void OnLand() => movingDown = false;

    public override void PartStart() => weaponManager = PlayerWeaponManager.instance;

    // add critical chance on homing shot
    public override void OnHomingShot()
    {
        satActive = true;
        if (!satActive && movingDown)
        weaponManager.criticalHitModifiers.Add(50f);
    }

    // remove critical chance on normal shot
    public override void OnWeaponFire()
    {
        if (satActive)
        weaponManager.criticalHitModifiers.Remove(50f);
    }
}
