using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_TyrantsFlight : BodyPartClass
{
    // while moving up relative gravity is lowered by 50%
    PlayerController player; // instance of player controller

    public override void PartStart()
    {
        // set instance
        player = PlayerController.instance;
    }

    public override void OnMoveUp()
    {
        // set player instance of gravity to 50% while moving up
        player.gravityUpMultiplier = 0.5f;
    }     

    public override void OnMoveDown()
    {
        // set player instance of gravity to 50% while moving up
        player.gravityUpMultiplier = 1f;
    }   
}
