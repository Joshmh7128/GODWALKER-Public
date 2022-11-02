using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_MovementSpeed : Effect
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
        switch(effectStage)
        {
            case 0:
                // reset speed
                playerController.moveSpeed = fearManager.basePlayerSpeed;
                break;

            case 1:
                // halve the movement speed
                playerController.moveSpeed = fearManager.basePlayerSpeed / 2;
                break;
                
            case 2:
                // halve the movement speed
                playerController.moveSpeed = fearManager.basePlayerSpeed / 3;
                break;

            case 3:
                // halve the movement speed
                playerController.moveSpeed = fearManager.basePlayerSpeed / 4;
                break;
        }
    }
}
