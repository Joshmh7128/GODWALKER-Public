using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ProjectileSpeed : Effect
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
                playerController.weaponManager.currentWeapon.bulletPrefab.GetComponent<PlayerProjectileScript>().speed = 200;
                break;

            case 1:
                playerController.weaponManager.currentWeapon.bulletPrefab.GetComponent<PlayerProjectileScript>().speed = 50;
                break;

            case 2:
                playerController.weaponManager.currentWeapon.bulletPrefab.GetComponent<PlayerProjectileScript>().speed = 12.5f;
                break;

            case 3:
                playerController.weaponManager.currentWeapon.bulletPrefab.GetComponent<PlayerProjectileScript>().speed = 3.25f;
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
