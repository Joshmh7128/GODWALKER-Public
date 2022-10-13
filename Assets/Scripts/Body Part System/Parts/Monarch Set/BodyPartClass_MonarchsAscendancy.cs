using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsAscendancy : BodyPartClass
{
    // this is a conglomerate of all the monarch parts into one part

    // while moving down relative gravity is lowered by 50% (really 25% for proper effect)
    PlayerController player; // instance of player controller

    // while moving down whenever a homing shot deals damage instantly reload
    PlayerWeaponManager weaponManager;
    bool intimidationActive;
    bool satActive, movingDown;
    bool willActive;
    bool discrepancyActive;

    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
        player.gravityMidairMultiplier = 0; // reset this so that we don't float after picking up the midair version of this 
        player.gravityUpMultiplier = 1;
        weaponManager = PlayerWeaponManager.instance;
    }

    public override void OnMoveUp()
    {
        // reset player gravity down multiplier
        player.gravityDownMultiplier = 1f; 
        intimidationActive = false;
        movingDown = false;
        // deactivate ability
        willActive = false;
        discrepancyActive = false;
    }

    public override void OnMoveDown()
    {
        // set player instance of gravity to 50% while moving down
        player.gravityDownMultiplier = 0.2f;
        intimidationActive = true;
        movingDown = true;
        // ability is active 
        willActive = true;
        discrepancyActive = true;
    }

    public override void OnLand()
    {
        player.gravityDownMultiplier = 1;

        intimidationActive = false;
        movingDown = false;
        // deactivate ability
        willActive = false;
        discrepancyActive = false;
    }

    // remove critical chance on normal shot
    public override void OnWeaponFire()
    {

        RequestHoming();

        if (satActive)
            weaponManager.criticalHitModifiers.Remove(50f);
    }

    public override void OnDoubleShot()
    {
        RequestHoming();
    }

    public override void OnHomingShotDamage()
    {
        // if we are active 
        if (intimidationActive)
        {
            int c = Random.Range(0, 100);
            if (c <= 90)
                weaponManager.currentWeapon.Reload(true);
        }

        // roll for success
        int d = Random.Range(0, 100);
        if (d <= 90 && discrepancyActive)
        {
            weaponManager.currentWeapon.requestHomingShot = true;
        }
    }

    // add critical chance on homing shot
    public override void OnHomingShot()
    {
        satActive = true;
        if (!satActive && movingDown)
            weaponManager.criticalHitModifiers.Add(50f);
    }
    
    // weapon homing shot request
    void RequestHoming()
    {
        if (willActive)
            weaponManager.currentWeapon.requestHomingShot = true;
    }
}
