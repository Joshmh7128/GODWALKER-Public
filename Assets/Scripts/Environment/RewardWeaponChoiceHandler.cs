using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardWeaponChoiceHandler : MonoBehaviour
{
    [SerializeField] int currentChildCount, highestChildCount; // what is the highest child count we've had?

    private void FixedUpdate()
    {        
        // if at any point our child count is less than our highest childcount, destroy all the weapons the player didn't choose (they will be children)
        if (currentChildCount < highestChildCount)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Weapon_Item>().DestroyItem();
            }
        }

        // then set our highest count after so that we only catch decreases in the amount
        currentChildCount = transform.childCount;

        if (currentChildCount > highestChildCount)
            highestChildCount = currentChildCount;
    }
}
