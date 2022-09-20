using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_MonarchsFloat : BodyPartClass
{
    // while moving down relative gravity is lowered by 50% (really 25% for proper effect)
    PlayerController player; // instance of player controller

    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
    }

    public override void OnMoveUp()
    {
        // reset player gravity down multiplier
        player.gravityDownMultiplier = 1f;
    }

    public override void OnMoveDown()
    {
        // set player instance of gravity to 50% while moving down
        player.gravityDownMultiplier = 0.3f; 
    }
}