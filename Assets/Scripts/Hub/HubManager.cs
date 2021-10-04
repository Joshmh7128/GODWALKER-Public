using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    // player spawn tracking
    [SerializeField] GameData gameData;
    // this script saves and tracks all the upgrades the player has obtained
    // currency stored in the hub
    public float hubMineralAmount;
    public float hubGemAmount;
    public float hubBugPartAmount;
    public float hubSpecialAmount;
    public float dropPodAmmoAmount;
    public float storageUpgradeFactor; // starts at 2x
    // droppod manager
    public DroppodManager droppodManager;
    // player
    public PlayerController playerController;

    // awake is called before the first frame update
    void Awake()
    {
        // make sure we don't destroy on load
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // find our droppodmanager
        if (droppodManager == null)
        {
            droppodManager = GameObject.Find("Drop Pod").GetComponent<DroppodManager>();
        }

        // find our player
        if (playerController == null)
        {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        // save progress
        SaveProgress();
    }

    // call this from any other script to save the progress upon landing in the hub
    public void SaveProgress()
    {
        // save on load
        gameData.SaveAll();
        // update our progress
        UpdateProgress();
        // debug
        Debug.Log("Progress Saved.");
    }

    // call this internally to update our progress
    void UpdateProgress()
    {
        // update our data to match the save file
        hubMineralAmount = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.hubMinerals];
        hubGemAmount = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.hubGems];
        hubBugPartAmount = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.hubBugParts];
        hubSpecialAmount = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.hubSpecials];
        dropPodAmmoAmount = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.dropPodAmmoAmount];
        droppodManager.ammoMax = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.dropPodAmmoMax];
        droppodManager.gemMax = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.dropPodGemMax];
        droppodManager.mineralMax = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.dropPodMineralMax];
        droppodManager.bugPartMax = gameData.saveData.SaveDataFloatArray[(int)GameData.SaveDataTypes.dropPodBugPartMax];
    }
}
