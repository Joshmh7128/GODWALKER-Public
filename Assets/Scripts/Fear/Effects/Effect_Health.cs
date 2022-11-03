using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Health : Effect
{
    PlayerStatManager playerStatManager;
    FearManager fearManager;

    public override void StartEffect()
    {
        playerStatManager = PlayerStatManager.instance;
        fearManager = FearManager.instance;
    }

    // apply our effect
    public override void ApplyEffect()
    {
        playerStatManager = PlayerStatManager.instance;
        fearManager = FearManager.instance;

        switch (effectStage)
        {
            case 0:
                // set to base hp
                playerStatManager.maxHealth = fearManager.basePlayerMaxHealth;
                break;

            case 1:
                // set to 1 hp
                playerStatManager.maxHealth = 5;
                playerStatManager.health = 5;
                break;        
            
            case 2:
                // set to 1 hp
                playerStatManager.maxHealth = 1;
                playerStatManager.health = 1;
                break;
        }

        // then set effect info
        effectInfo = effectInfos[effectStage];
    }
}
