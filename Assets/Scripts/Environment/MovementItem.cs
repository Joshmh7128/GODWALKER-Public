using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementItem : ItemClass
{
    public string itemName; // our item name
    public string itemInfo; // our item information
    [SerializeField] GameObject vfx;

    public enum Abilities
    {
        dash, 
    }

    // which ability do we want to activate
    [SerializeField] Abilities targetAbility;

    private void Update()
    {
        // process our pickup
        ProcessPickup();
    }

    // on pickup
    void ProcessPickup()
    {
        // if we can grab and can pickup
        if (canGrab & Input.GetKeyDown(KeyCode.E))
        {
            // unlock our ability
            PlayerMovementAbilityManager.instance.movementAbilites[(int)targetAbility] = true;
            Instantiate(vfx, transform.position, Quaternion.identity, null);
            Destroy(gameObject);
        }
    }
}
