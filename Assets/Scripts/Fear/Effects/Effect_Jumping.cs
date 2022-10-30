using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Jumping : Effect 
{
    PlayerController playerController; // our player controller
    FearManager fearManager;

    public override void StartEffect()
    {
        playerController = PlayerController.instance;
        fearManager = FearManager.instance;

        maxStage = 2; // we have 3 total stages
    }

    // apply our effect
    public override void ApplyEffect()
    {
        switch (effectStage)
        {
            case 0:
                // set to double jump
                playerController.maxJumps = 2;
                break;

            case 1:
                // single jump
                playerController.maxJumps = 1;
                break;

            case 2:
                // no jump
                playerController.maxJumps = 0;
                break;
        }
    }
}
