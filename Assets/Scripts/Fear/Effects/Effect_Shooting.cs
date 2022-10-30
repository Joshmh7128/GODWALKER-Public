using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Shooting : Effect
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
                // set to first gun
                playerController.weaponManager.currentWeaponInt = 0;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case 1:
                // set to second gun
                playerController.weaponManager.currentWeaponInt = 1;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case 2:
                // set to third gun
                playerController.weaponManager.currentWeaponInt = 2;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;

            case 3:
                // set to no gun
                playerController.weaponManager.currentWeaponInt = 3;
                playerController.weaponManager.UpdateCurrentWeapon();
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
