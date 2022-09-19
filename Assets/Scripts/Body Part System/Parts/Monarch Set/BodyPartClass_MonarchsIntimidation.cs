using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsIntimidation : BodyPartClass
{
    // while moving down whenever a homing shot deals damage instantly reload

    PlayerWeaponManager weaponManager;
    bool active; 

    public override void PartStart()
    {
        // get instance
        weaponManager = PlayerWeaponManager.instance;
    }

    // set active
    public override void OnMoveDown()
    {
        active = true;
    }

    public override void OnLand()
    {
        active = false;
    }

    public override void OnMoveUp()
    {
        active = false;
    }

    public override void OnHomingShotDamage()
    {
        // if we are active 
        if (active)
        {
            int c = Random.Range(0, 100);
            if (c <= 90)
            weaponManager.currentWeapon.Reload(true);
        }
    }
}
