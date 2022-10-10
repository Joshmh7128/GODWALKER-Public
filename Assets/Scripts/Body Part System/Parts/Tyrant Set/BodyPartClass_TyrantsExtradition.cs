using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsExtradition : BodyPartClass
{
    // while moving up relative gravity is lowered by 50% ( really 25% for proper effect)
    PlayerController player; // instance of player controller
    PlayerWeaponManager weaponManager;
    // while moving up gain +20% critical chance
    bool risingActivated;
    bool retributionActive; // is this active? when is it active?
    bool batActive; // is this active? when is it active?

    bool dichActive, movingUp; // is this active?
    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
        player.gravityMidairMultiplier = 0; // reset this so that we don't float after picking up the midair version of this 
        player.gravityDownMultiplier = 1;

        weaponManager = PlayerWeaponManager.instance;
    }

    // whenever we double fire, check if we are active
    public override void OnDoubleShot()
    {
        // is this active?
        int c = Random.Range(0, 100);
        if (c <= 30)
        {
            dichActive = true;
        }

        if (retributionActive)
        {
            int d = Random.Range(0, 100);
            if (d <= 25)
                weaponManager.criticalHitModifiers.Add(25f);
        }
    }

    // if we are active on weapon fire...
    public override void OnWeaponFire()
    {
        if (dichActive && movingUp)
        {
            // request then fire a double shot
            weaponManager.currentWeapon.requestDoubleShot = true;
            // then make us inactive
            dichActive = false;
        }

        // if this ability is active
        if (batActive)
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
    // on move up add 20 to the critical modifiers on the weapon manager
    public override void OnMoveUp()
    {
        batActive = true;
        movingUp = true;
        player.gravityUpMultiplier = 0.3f;

        if (!risingActivated)
            weaponManager.criticalHitModifiers.Add(20f);
        risingActivated = true; retributionActive = true;
    }

    // when we move down remove the 20 from the weapon manager's 
    public override void OnMoveDown()
    {
        batActive = false;
        movingUp = false;
        if (risingActivated)
            weaponManager.criticalHitModifiers.Remove(20f);
        risingActivated = false;

        player.gravityUpMultiplier = 1f;

        // if we are moving down and it ability is still active
        if (retributionActive)
        {
            // remove our additional chance
            weaponManager.criticalHitModifiers.Remove(25f);
            // then set it to inactive
            retributionActive = false;
        }
    }
}
