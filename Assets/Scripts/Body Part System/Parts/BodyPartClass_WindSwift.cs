using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_WindSwift : BodyPartClass
{
    // windswift increases our firerate by 20% while sprinting
    float firerateReduction; // how much to reduce the firerate by
    PlayerWeaponManager weaponManager;
    bool sprinting;

    // use our part start in this part
    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // we apply our new firerate when we are sprinting
    public override void OnSprint()
    {
        sprinting = true;
        // calculate firerate mod
        CalculateFirerate(); 
        // apply our firerate mod
        weaponManager.currentWeapon.firerateMod = -firerateReduction;
    }

    public override void OffSprint()
    {
        sprinting = false;      
        // apply our fire rate mod
        weaponManager.currentWeapon.firerateMod = 0;
    }

    public override void OnWeaponSwap()
    {
        // recalculate our amount
        CalculateFirerate();
        // then if we are sprinting, apply the firerate
        if (sprinting)
        {
            weaponManager.currentWeapon.firerateMod = -firerateReduction;
        }
    }

    void CalculateFirerate()
    {
        // get the firerate of the current weapon
        float firerate;
        firerate = weaponManager.currentWeapon.firerate;
        // then set the firerate reduction to be 20% of that
        firerateReduction = firerate * 0.2f;
    }
}
