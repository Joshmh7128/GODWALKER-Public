using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_RoyalsVault : BodyPartClass
{
    // while midair relative gravity is lowered by 50% ( really 25% for proper effect)
    PlayerController player; // instance of player controller

    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
    }

    public override void OnMoveMidair()
    {
        // set player instance of gravity to 50% while moving up
        player.gravityMidairMultiplier = 0.3f;
    }
}
