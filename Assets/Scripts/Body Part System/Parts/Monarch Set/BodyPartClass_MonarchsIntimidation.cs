using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsIntimidation : BodyPartClass
{
    // while moving down whenever a homing shot deals damage instantly reload

    PlayerWeaponManager weaponManager;

    public override void PartStart()
    {
        // get instance
        weaponManager = PlayerWeaponManager.instance;
    }

    public override void OnHomingShotDamage()
    {
        weaponManager.currentWeapon.Reload(true);
    }
}
