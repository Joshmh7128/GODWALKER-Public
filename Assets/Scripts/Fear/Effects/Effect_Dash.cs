using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Dash : Effect
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
                // set player dash to full speed
                playerController.canDash = true;
                playerController.dashTimeMax = playerController.dashTimeLongMax;
                break;

            case 1:
                // set dash to inactive
                playerController.canDash = false;
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
