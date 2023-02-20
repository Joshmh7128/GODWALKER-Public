using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatAbility_Item : ItemClass
{
    // the ability we're working with
    public PlayerCombatAbility ability;
    public string itemName, itemInfo; // our name and info
    public Sprite abilityIcon;

    private void Awake()
    {
        itemName = ability.abilityName;
        itemInfo = ability.abilityInfo;
    }


    private void Update()
    {
        // if we can pick it up
        if (canGrab)
        { 
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerCombatAbilityManager.instance.PickupAbility(true, this);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayerCombatAbilityManager.instance.PickupAbility(false, this);
            }

        }
    }
}
