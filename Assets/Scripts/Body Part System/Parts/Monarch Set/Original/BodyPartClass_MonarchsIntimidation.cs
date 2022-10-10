using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsIntimidation : BodyPartClass
{
    // while moving down whenever a homing shot deals damage instantly reload

    PlayerWeaponManager weaponManager;
    bool intimidationActive; 

    public override void PartStart() => weaponManager = PlayerWeaponManager.instance;

    // set active
    public override void OnMoveDown() => intimidationActive = true;

    public override void OnLand() => intimidationActive = false;

    public override void OnMoveUp() => intimidationActive = false;

    public override void OnHomingShotDamage()
    {
        // if we are active 
        if (intimidationActive)
        {
            int c = Random.Range(0, 100);
            if (c <= 90)
            weaponManager.currentWeapon.Reload(true);
        }
    }
}
