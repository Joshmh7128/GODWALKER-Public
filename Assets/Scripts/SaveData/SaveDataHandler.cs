using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataHandler : MonoBehaviour
{
    /// this script saves and loads objects when called

    public static SaveDataHandler instance; // our instance
    private void Awake()
    {
        instance = this;

        // then set our live data
        liveData = new SaveData();
        // then load our game
        LoadGame();

    }

    // the live save data from our game. we set our data at the start of play from our file, and write this data to the save file when we save
    public static SaveData liveData; 

    // write our liveData to our saveData
    public void SaveGame()
    {
        // convert data
        SaveData save = liveData;
        string json = JsonUtility.ToJson(save);

        // save data based on application data
        System.IO.File.WriteAllText(Application.persistentDataPath, "/SaveGames/" + Application.version + "SaveData.json");
    }

    // sets our liveData to our save data if that save data exists
    public void LoadGame()
    {
        // check for data
        try
        {
            // get our save data
            string rawData = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveGames/" + Application.version + "SaveData.json");
            SaveData json = JsonUtility.FromJson<SaveData>(rawData);

            // set our live data to our json
            liveData = json;

            // debug that we loaded
            Debug.Log("Save Data loaded.");
        }
        catch
        {
            // if there is no save data found, it is probably because we are on a new version. if this is the case, wipe the data clean!
            Debug.Log("No Save Data Found! Resetting file to default and autosaving...");
            // set our livedata to a new SaveData
            liveData = new SaveData();
            SaveGame();
        }
    }

}
