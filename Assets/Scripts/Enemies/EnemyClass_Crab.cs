using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Crab : EnemyClass
{
    [SerializeField] LocalIKHandler hurtHandler;

    // override our get hurt extend since we're not doing anything differently that requires an override of the main gethurt class
    public override void GetHurtExtension()
    {
        // kick our body
        hurtHandler.KickLookPos(30f);
    }

    public override void SetLevelStats()
    {
        // set our health
        maxHealth = (level + level * 0.1f) * 50f; // crabs have 50hp per level
        health = maxHealth;
        // set our damage
        // damage = level * (2 + level * 0.1f); // this is a standard curve
        damage = 10; // set our basic damage
    }

}
