using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsWill : BodyPartClass
{
    // While Moving Midair Shots have an 80% chance to be Homing Shots
    bool active; // is this active?
    PlayerWeaponManager weaponManager; // our weapon manager

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    public override void OnWeaponFire()
    {
        RequestHoming();
    }

    public override void OnDoubleShot()
    {
        RequestHoming();
    }

    void RequestHoming()
    {
        if (active)
            weaponManager.currentWeapon.requestHomingShot = true;
    }

    public override void OnMoveMidair()
    {
        // ability is active 
        active = true;
    }

    public override void OnLand()
    {
        // deactivate ability
        active = false;
    }   
}
