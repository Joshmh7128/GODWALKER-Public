using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_IceblinkOnset : BodyPartClass
{
    // All shots are now homing, lol
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
