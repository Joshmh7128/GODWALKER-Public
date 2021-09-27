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
        }
    }    
    
    // restocks the player
    public void RestockPlayer()
    {
        hubManager.playerController.ammoAmount = hubManager.playerController.ammoMax;
    }
}
