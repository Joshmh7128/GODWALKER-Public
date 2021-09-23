using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class GameData : MonoBehaviour
{
    // GameData reads, checks, and saves all of our game information
    public SaveData saveData;
    public SaveData saveDataJson;
    [SerializeField] HubManager hubManager;

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
        hubSpecial,

        // maximum value so we can quickly create an array
        saveDataEnumMax
    }

    // Save All saves all data to the JSON file
    public void SaveAll()
    {
        Debug.Log("Save All Called. Saving all data in Json format to: " + Application.persistentDataPath + "/gamedata.data");

        if (File.Exists(Application.persistentDataPath + "/gamedata.data"))
        {
            // File exists. Read it. Update it.
            string fileContents = File.ReadAllText(Application.persistentDataPath + "/gamedata.data");
            // get the Json in to a class. This is the data that has been created from our previous saves
            saveDataJson = JsonUtility.FromJson<SaveData>(fileContents);
            // from the previous play save, let's go ahead and import the data
            saveData = saveDataJson;
            // and there we have it! the Json data has been imported in to Unity to be read
        }
        else
        {
            // update our save data
            // access our save data and add the values
            saveData.SaveDataFloatArray[(int)SaveDataTypes.hubMinerals] = hubManager.hubMineralAmount;
            saveData.SaveDataFloatArray[(int)SaveDataTypes.hubGems] = hubManager.hubGemAmount;
            saveData.SaveDataFloatArray[(int)SaveDataTypes.hubBugParts] = hubManager.hubBugPartAmount;
            saveData.SaveDataFloatArray[(int)SaveDataTypes.hubSpecial] = 0f;
            // If our save File does not exist, create a new save file from our current values
            string jsonString = JsonUtility.ToJson(saveData);
            // write JSON to file
            File.WriteAllText(Application.persistentDataPath + "/gamedata.data", jsonString);
        }
    }
}