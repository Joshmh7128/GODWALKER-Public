using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubLink : MonoBehaviour
{
    // our hub manager
    public HubManager hubManager;
    public HubUIManager hubUIManager;

    // Update is called once per frame
    void Update()
    {
        if (hubManager == null)
        {
            hubManager = GameObject.Find("Hub Manager").GetComponent<HubManager>();
        }
    }

    // restocks the dropship
    public void RestockDropship()
    {
        // check the values and if we can afford it
        if ((hubManager.hubMineralAmount > hubUIManager.resupplyDropshipAmmoCost) && (hubManager.hubGemAmount > hubUIManager.resupplyDropshipAmmoCost))
        {
            // deduct the values
            hubManager.hubMineralAmount -= hubUIManager.resupplyDropshipAmmoCost;
            hubManager.hubGemAmount -= hubUIManager.resupplyDropshipAmmoCost;
            // restock
            hubManager.droppodManager.ammoAmount = hubManager.droppodManager.ammoMax;
            // refresh values
            hubUIManager.RefreshValues();
            // save
            hubManager.SaveProgress();
        }
    }    
    
    // restocks the player
    public void RestockPlayer()
    {
        hubManager.playerController.ammoAmount = hubManager.playerController.ammoMax;
        // save
        hubManager.SaveProgress();
    }

    // upgrades the dropship
    public void UpgradeDropShipStorage(string upgradeType)
    {
        // get the value we want to increase
        if (upgradeType == "ammo")
        {
            // check currency
            if (hubManager.hubBugPartAmount >= hubManager.droppodManager.ammoUpgradeCost)
            {
                // spend currency
                hubManager.hubBugPartAmount -= hubManager.droppodManager.ammoUpgradeCost;
                // increase it and decrease our bug parts from storage
                hubManager.droppodManager.ammoMax = hubManager.droppodManager.ammoMax * 2;
            }
        }             
        
        // get the value we want to increase
        if (upgradeType == "minerals")
        {
            // check currency
            if (hubManager.hubBugPartAmount >= hubManager.droppodManager.mineralUpgradeCost)
            {
                // spend currency
                hubManager.hubBugPartAmount -= hubManager.droppodManager.mineralUpgradeCost;
                // increase it and decrease our bug parts from storage
                hubManager.droppodManager.mineralMax = hubManager.droppodManager.mineralMax * 2;
            }
        }        
        
        // get the value we want to increase
        if (upgradeType == "gems")
        {
            // check currency
            if (hubManager.hubBugPartAmount >= hubManager.droppodManager.gemUpgradeCost)
            {
                // spend currency
                hubManager.hubBugPartAmount -= hubManager.droppodManager.gemUpgradeCost;
                // increase it and decrease our bug parts from storage
                hubManager.droppodManager.gemMax = hubManager.droppodManager.gemMax * 2;
            }
        }     
        
        // get the value we want to increase
        if (upgradeType == "bugparts")
        {
            // check currency
            if (hubManager.hubBugPartAmount >= hubManager.droppodManager.bugPartUpgradeCost)
            {
                // spend currency
                hubManager.hubBugPartAmount -= hubManager.droppodManager.gemUpgradeCost;
                // increase it and decrease our bug parts from storage
                hubManager.droppodManager.bugPartMax = hubManager.droppodManager.bugPartMax * 2;
            }
        }

        // save
        hubManager.SaveProgress();
    }
}
