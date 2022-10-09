using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxHealthIncrease_Item : ItemClass
{
    [SerializeField] float maxHealthIncrease; // how much does this item increase max health?
    string infoString; // our info
    [SerializeField] ItemUIHandler uiHandler; // our UI handler

    // our instance
    PlayerStatManager playerStatManager;

    private void Start()
    {
        // get instance
        playerStatManager = PlayerStatManager.instance;

        // set our stats
        SetStats();
    }

    void SetStats()
    {
        if (maxHealthIncrease != 0)
        {
            maxHealthIncrease = (int)Random.Range(1, 3) * 10;
        }

        infoString = "Increases Maximum HP by " + maxHealthIncrease;
        uiHandler.itemInfo.text = infoString;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        ProcessPickup();
    }

    // if we can grab this item
    void ProcessPickup()
    {
        // if we can grab and can pickup
        if (canGrab & Input.GetKey(KeyCode.E))
        {
            playerStatManager.AddMaxHealth(maxHealthIncrease);
        }
    }
}
