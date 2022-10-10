using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Crab : EnemyClass
{
    [SerializeField] LocalIKHandler hurtHandler;
    [SerializeField] float baseHealth; // the base health of this enemy
    [SerializeField] bool kicks; // do we apply a kick animation?

    public override void StartExtension()
    {
        // find and set our player as the look target
        if (hurtHandler != null)
        hurtHandler.lookTarget = PlayerController.instance.transform;
    }

    // override our get hurt extend since we're not doing anything differently that requires an override of the main gethurt class
    public override void GetHurtExtension()
    {
        // kick our body
        if (kicks)
        hurtHandler.KickLookPos(30f);
    }

    public override void SetLevelStats()
    {
        // set our level to the active arena
        level = ArenaManager.instance.activeArena.arenaLevel;

        // set our health
        maxHealth = (level + level * 0.15f) * baseHealth; // crabs have 100hp per level
        health = maxHealth;
        // set our damage
        // damage = level * (2 + level * 0.1f); // this is a standard curve
        damage = 10; // set our basic damage
    }

}
