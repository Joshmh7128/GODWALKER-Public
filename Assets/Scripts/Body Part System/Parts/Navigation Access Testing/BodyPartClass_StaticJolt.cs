using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_StaticJolt : BodyPartClass
{
    // setup instances
    PlayerWeaponManager weaponManager;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // when we pick this up request a shock shot
    public override void OnBodyPartPickup()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
    }

    // whenever we shoot request a shock shot
    public override void OnWeaponFire()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
    }

    public override void OnWeaponSwap()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
    }

}
