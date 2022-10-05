using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsBattalion : BodyPartClass
{
    // while moving up shots have a 60% to double shoot

    bool active; // is this active? when is it active?
    public override void OnMoveUp() { active = true; }
    public override void OnMoveDown() { active = false; }

    // our insances
    PlayerWeaponManager weaponManager;
    PlayerBodyPartManager bodyPartManager;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;
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
        weaponManager.currentWeapon.Fire(true);
        // send out the call that we're double-shotting
        bodyPartManager.CallParts("OnDoubleShot");
    }

}
