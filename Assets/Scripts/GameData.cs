using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class GameData : MonoBehaviour
{
    // GameData reads, checks, and saves all of our game information
    public SaveData saveData;
    [SerializeField] HubManager hubManager;
    public bool gameStart = false;

    // make sure we do not get destroyed
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    // create an enum in which all of our saved values are stored
    public enum SaveDataTypes
    {
        // hub materials gathered in between runs
        hubMinerals,
        hubGems,
        hubBugParts,
        hubSpecials,
        dropPodAmmoAmount,
        dropPodAmmoMax,
        dropPodMineralMax,
        dropPodGemMax,
        dropPodBugPartMax,
        storageUpgradeFactor,
        playerAmmoMax,
        playerMineralMax,
        playerGemMax,

        // maximum value so we can quickly create an array
        saveDataEnumMax
    }

    // Save All saves all data to the JSON file
    public void SaveAll()
    {
        Debug.Log("Save All Called. Saving all data in Json format to: " + Application.persistentDataPath + "/gamedata.data");

        if (File.Exists(Application.persistentDataPath + "/gamedata.data"))
        {
            if (gameStart == false)
            {
                // File exists. Read it. Apply it to our instance of the game
                string fileContents = File.ReadAllText(Application.persistentDataPath + "/gamedata.data");
                // get the Json in to a class. This is the data that has been created from our previous saves
                saveData = JsonUtility.FromJson<SaveData>(fileContents);
                // and there we have it! the Json data has been imported in to Unity to be read
                gameStart = true;
            }
            else // if we're saving again...
            {
                SaveDataUpdate();
            }
        }
        else
        {
            SaveDataUpdate();
        }
    }

    void SaveDataUpdate()
    {
        // update our save data
        // access our save data and add the values
        saveData.SaveDataFloatArray[(int)SaveDataTypes.hubMinerals] = hubManager.hubMineralAmount;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.hubGems] = hubManager.hubGemAmount;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.hubBugParts] = hubManager.hubBugPartAmount;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.hubSpecials] = hubManager.hubSpecialAmount;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.dropPodAmmoAmount] = hubManager.dropPodAmmoAmount;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.dropPodAmmoMax] = hubManager.droppodManager.ammoMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.dropPodGemMax] = hubManager.droppodManager.gemMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.dropPodMineralMax] = hubManager.droppodManager.mineralMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.dropPodBugPartMax] = hubManager.droppodManager.bugPartMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.storageUpgradeFactor] = hubManager.storageUpgradeFactor;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.playerAmmoMax] = hubManager.playerController.ammoMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.playerGemMax] = hubManager.playerController.gemMax;
        saveData.SaveDataFloatArray[(int)SaveDataTypes.playerMineralMax] = hubManager.playerController.mineralMax;
        // If our save File does not exist, create a new save file from our current values
        string jsonString = JsonUtility.ToJson(saveData);
        // write JSON to file
        File.WriteAllText(Application.persistentDataPath + "/gamedata.data", jsonString);
    }
}