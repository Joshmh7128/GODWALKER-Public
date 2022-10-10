using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsWill : BodyPartClass
{
    // While Moving Down Shots have an 80% chance to be Homing Shots
    bool willActive; // is this active?
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
        if (willActive)
            weaponManager.currentWeapon.requestHomingShot = true;
    }

    public override void OnMoveDown()
    {
        // ability is active 
        willActive = true;
    }

    public override void OnLand()
    {
        // deactivate ability
        willActive = false;
    }

    public override void OnMoveUp()
    {
        // deactivate ability
        willActive = false;
    }
}
