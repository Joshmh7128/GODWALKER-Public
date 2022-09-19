using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsDiscrepancy : BodyPartClass
{
    // while moving down whenever a homing shot deals damage there is a 90% 
    // chance your next shot will be a homing shot as well
    PlayerWeaponManager weaponManager; // instance

    // get our weapon manager instance
    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // whenever a homing shot deals damage
    public override void OnHomingShotDamage()
    {
        // roll for success
        int c = Random.Range(0, 100);
        if (c <= 90)
        {
            weaponManager.currentWeapon.requestHomingShot = true;
        }
    }
}
