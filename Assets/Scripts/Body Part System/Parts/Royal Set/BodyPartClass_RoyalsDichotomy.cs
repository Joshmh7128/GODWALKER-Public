using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsDichotomy : BodyPartClass
{
    /// while moving up if you fire a double shot there is a +30% chance that your next 
    /// shot will also be a double shot
    /// 

    PlayerWeaponManager weaponManager;
    bool active, midair; // is this active?

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    public override void OnMoveMidair() => midair = true;
    public override void OnLand() => midair = false;

    // whenever we double fire, check if we are active
    public override void OnDoubleShot()
    {
        // is this active?
        int c = Random.Range(0, 100);
        if (c <= 30)
        {
            active = true;
        }
    }

    // if we are active on weapon fire...
    public override void OnWeaponFire()
    {
        if (active && midair)
        {
            // request then fire a double shot
            weaponManager.currentWeapon.FireDoubleShot();
            // then make us inactive
            active = false;
        }
    }

}
