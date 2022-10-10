using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsRising : BodyPartClass
{
    // while moving up game +20% critical chance

    PlayerWeaponManager weaponManager;
    bool risingActivated;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // on move up add 20 to the critical modifiers on the weapon manager
    public override void OnMoveUp()
    {
        if (!risingActivated)
        weaponManager.criticalHitModifiers.Add(20f);
        risingActivated = true;
    }

    // when we move down remove the 20 from the weapon manager's 
    public override void OnMoveDown()
    {
        if (risingActivated)
        weaponManager.criticalHitModifiers.Remove(20f);
        risingActivated = false;
    }

}
