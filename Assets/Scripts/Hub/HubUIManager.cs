using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubUIManager : MonoBehaviour
{
    // our hub manager
    HubManager hubManager;
    // resupply variables 
    public float resupplyDropshipAmmoCost;
    public Text resupplyDropshipAmmoCostTextGems;
    public Text resupplyDropshipAmmoCostTextMinerals;
    public Text resupplyDropshipPlayerCostTextGems;
    public Text resupplyDropshipPlayerCostTextMinerals;
    public float resupplyPlayerCost; // cost to resupply HP and Ammo
    // stored resources
    [SerializeField] List<Text> diegeticGemStoredAmount;
    [SerializeField] List<Text> diegeticMineralStoredAmount;
    [SerializeField] List<Text> diegeticBugPartStoredAmount;
    [SerializeField] List<Text> diegeticSpecialStoredAmount;
    [SerializeField] List<InteractionSpot> interactionSpots; // all the interaction spots in the scene
    // our diegetic UI elements
    private void Start()
    {
        // what is our hub manager
        if (hubManager == null)
        {
            hubManager = GameObject.Find("Hub Manager").GetComponent<HubManager>();
        }

        RefreshValues();
    }

    public void FixedUpdate()
    {
        RefreshValues();
    }

    // put all values we want to refresh in here
    public void RefreshValues()
    {
        // refresh our hub values
        foreach(Text text in diegeticGemStoredAmount)
        {
            text.text = hubManager.hubGemAmount.ToString();
        }       
        
        foreach(Text text in diegeticMineralStoredAmount)
        {
            text.text = hubManager.hubMineralAmount.ToString();
        } 
        
        foreach(Text text in diegeticBugPartStoredAmount)
        {
            text.text = hubManager.hubBugPartAmount.ToString();
        }       
        
        foreach(Text text in diegeticSpecialStoredAmount)
        {
            text.text = hubManager.hubSpecialAmount.ToString();
        }

        float localAmmoMax = hubManager.droppodManager.ammoMax;
        float localAmmoAmount = hubManager.droppodManager.ammoAmount;

        // to calculate how much this costs, for each ammo we multiple each value by ten, round down
        resupplyDropshipAmmoCost = Mathf.Round((localAmmoMax -= localAmmoAmount) *0.1f);
        resupplyDropshipAmmoCostTextGems.text = resupplyDropshipAmmoCost.ToString();
        resupplyDropshipAmmoCostTextMinerals.text = resupplyDropshipAmmoCost.ToString();
        // calculate the cost of resupplying the player's ammo and HP
        resupplyPlayerCost = (Mathf.Round(hubManager.playerController.playerMaxHP - hubManager.playerController.playerHP)) + (Mathf.Round(hubManager.playerController.ammoMax - hubManager.playerController.ammoAmount) * 0.5f);
        resupplyDropshipPlayerCostTextGems.text = resupplyPlayerCost.ToString();
        resupplyDropshipPlayerCostTextMinerals.text = resupplyPlayerCost.ToString();
        hubManager.playerResupplyCost = resupplyPlayerCost;
    }

    // Update runs every frame
    private void Update()
    {
        // what is our hub manager
        if (hubManager == null)
        {
            hubManager = GameObject.Find("Hub Manager").GetComponent<HubManager>();
        }

        
    }
}
