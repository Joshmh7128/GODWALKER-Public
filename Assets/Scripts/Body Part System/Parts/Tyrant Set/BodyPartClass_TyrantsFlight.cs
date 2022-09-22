using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsFlight : BodyPartClass
{
    // while moving up relative gravity is lowered by 50% ( really 25% for proper effect)
    PlayerController player; // instance of player controller

    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
        player.gravityMidairMultiplier = 0; // reset this so that we don't float after picking up the midair version of this 
        player.gravityDownMultiplier = 1;
    }

    // set player instance of gravity to 50% while moving up
    public override void OnMoveUp() => player.gravityUpMultiplier = 0.3f;

    // set player instance of gravity to 50% while moving up
    public override void OnMoveDown() => player.gravityUpMultiplier = 1f;
    
}
