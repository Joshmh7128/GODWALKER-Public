using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsIntimidation : BodyPartClass
{
    // while moving midair whenever a homing shot deals damage instantly reload

    PlayerWeaponManager weaponManager;
    bool active;

    public override void PartStart() => weaponManager = PlayerWeaponManager.instance;

    // set active
    public override void OnMoveMidair() => active = true;

    public override void OnLand() => active = false;

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
