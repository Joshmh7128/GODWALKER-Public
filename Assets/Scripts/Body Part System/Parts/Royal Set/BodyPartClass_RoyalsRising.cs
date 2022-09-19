using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsRising : BodyPartClass
{
    // while moving midair +20% critical chance

    PlayerWeaponManager weaponManager;
    bool activated;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // on move up add 20 to the critical modifiers on the weapon manager
    public override void OnMoveMidair()
    {
        if (!activated)
            weaponManager.criticalHitModifiers.Add(20f);
        activated = true;
    }

    // when we move down remove the 20 from the weapon manager's 
    public override void OnLand()
    {
        if (activated)
            weaponManager.criticalHitModifiers.Remove(20f);
        activated = false;
    }

}
