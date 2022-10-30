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

        maxStage = 3; // we have 4 total stages
    }

    // apply our effect
    public override void ApplyEffect()
    {
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
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxSlow;
                playerController.shieldUptimeMax = playerController.shieldUptimeMaxSlow;
                break;

            case 2:
                // set to third deflect
                playerController.canDeflect = true;
                playerController.shieldRechargeMax = playerController.shieldRechargeMaxVerySlow;
                playerController.shieldUptimeMax = playerController.shieldUptimeMaxVerySlow;
                break;

            case 3:
                // you cant deflect
                playerController.canDeflect = false;
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
