using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsBattalion : BodyPartClass
{
    // while moving midair shots have a 60% to double shoot

    bool active; // is this active? when is it active?
    public override void OnMoveMidair() => active = true;
    public override void OnLand() => active = false;

    // our insances
    PlayerWeaponManager weaponManager;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // whenever this weapon is fired, see if we should double-fire by calling fire again at 10% of the fire rate
    public override void OnWeaponFire()
    {
        // if this ability is active
        if (active)
        {   // roll to see if we perform it
            int c = Random.Range(0, 100);
            if (c <= 60)
                RunDoubleShoot();
        }
    }

    void RunDoubleShoot()
    {
        // fire again
        if (weaponManager.currentWeapon.currentMagazine > 0)
        {
            weaponManager.currentWeapon.FireDoubleShot();
        }
    }

}
