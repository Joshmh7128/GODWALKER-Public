using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Deflect : Effect
{
    PlayerController playerController; // our player controller
    FearManager fearManager;

    public override void StartEffect()
    {
        playerController = PlayerController.instance;
        fearManager = FearManager.instance;
    }

    // apply our effect
    public override void ApplyEffect()
    {
        playerController = PlayerController.instance;
        fearManager = FearManager.instance;

        switch (effectStage)
        {
            case 0:
                // set to best deflect
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxFast;
                playerController.shieldUptimeMax = playerController.shieldUptimeMaxFast;
                break;

            case 1:
                // set to second deflect
                playerController.canDeflect = false;
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
