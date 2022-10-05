using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsRetribution : BodyPartClass
{
    // while moving up double shots have an increase 25% critical hit chance

    bool active; // is this active? when is it active?
    public override void OnMoveUp() { active = true; }
    public override void OnMoveDown() 
    {
        // if we are moving down and it ability is still active
        if (active)
        {
            // remove our additional chance
            weaponManager.criticalHitModifiers.Remove(25f);
            // then set it to inactive
            active = false;
        }
    }

    // instances
    PlayerWeaponManager weaponManager;

    public override void PartStart()
    {
        // setup instance
        weaponManager = PlayerWeaponManager.instance;
    }

    // apply our additional critical hit chance if we succeed the check while moving up
    public override void OnDoubleShot()
    {
        if (active)
        {
            int c = Random.Range(0, 100);
            if (c <= 25)
                weaponManager.criticalHitModifiers.Add(25f);
        }
    }
}
