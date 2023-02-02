using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardWeaponChoiceMonitor : MonoBehaviour
{
    int highestChildCount; // what is the highest child count we've had?

    private void FixedUpdate()
    {
        highestChildCount = transform.childCount;

        // if at any point our child count is less than our highest childcount, destroy all the weapons the player didn't choose (they will be children)
        if (transform.childCount > highestChildCount)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Weapon_Item>().DestroyItem();
            }
        }
    }
}
