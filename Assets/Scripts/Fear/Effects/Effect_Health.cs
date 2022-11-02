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

        maxStage = 2; // we have 3 total stages
    }

    // apply our effect
    public override void ApplyEffect()
    {
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
