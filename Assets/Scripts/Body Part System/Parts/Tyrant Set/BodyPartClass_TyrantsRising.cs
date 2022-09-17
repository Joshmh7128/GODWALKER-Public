using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsRising : BodyPartClass
{
    // while moving up game +20% critical chance

    PlayerWeaponManager weaponManager;

    public override void PartStart()
    {
        weaponManager = PlayerWeaponManager.instance;
    }

    // on move up add 20 to the critical modifiers on the weapon manager
    public override void OnMoveUp()
    {
        weaponManager.criticalHitModifiers.Add(20f);
    }

    // when we move down remove the 20 from the weapon manager's 
    public override void OnMoveDown()
    {
        weaponManager.criticalHitModifiers.Remove(20f);
    }

}
