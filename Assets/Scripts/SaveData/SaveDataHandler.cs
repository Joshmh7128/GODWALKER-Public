using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveDataHandler : MonoBehaviour
{
    /// this script saves and loads objects when called

    public static SaveDataHandler instance; // our instance

    // sets the file name to the current save data of the application version
    string filename; // is set in the Awake function

    private void Awake()
    {
        // set our file name before we begin any other operations
        filename = Application.version.ToString() + "SaveData.json";

        // set up our instance
        instance = this;

        // then set our live data
        liveData = new SaveData();

        // then load our game into the live data
        LoadGame();

        Debug.Log(Application.persistentDataPath);
    }

    // the live save data from our game. we set our data at the start of play from our file, and write this data to the save file when we save
    public SaveData liveData;

    // our debug output
    [SerializeField] Text debugOutput;

    // write our liveData to our saveData
    public void SaveGame()
    {

        // check to make sure our directory exists
        if (!Directory.Exists(Application.persistentDataPath))
        {
            // create it if we need to
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        // convert data
        SaveData save = liveData;
        string json = JsonUtility.ToJson(save);

        // save data based on application data
        File.WriteAllText(Path.Combine(Application.persistentDataPath + filename), json);

        Debug.Log(Application.persistentDataPath);
        // debug
        Debug.Log("Saving data to " + Application.persistentDataPath + filename);
        debugOutput.text = "Game Saved.";
        StartCoroutine(ClearDebug());
    }

    // sets our liveData to our save data if that save data exists
    public void LoadGame()
    {
        // check for data
        try
        {
            // get our save data
            string rawData = File.ReadAllText(Application.persistentDataPath + filename);
            SaveData json = JsonUtility.FromJson<SaveData>(rawData);

            // set our live data to our json
            liveData = json;

            // debug that we loaded
            Debug.Log("Save Data loaded.");
            debugOutput.text = "Save Data loaded.";
            StartCoroutine(ClearDebug());
        }
        catch
        {
            // if there is no save data found, it is probably because we are on a new version. if this is the case, wipe the data clean!
            Debug.Log("No Save Data Found! Resetting file to default and autosaving...");
            // set our livedata to a new SaveData
            liveData = new SaveData();
            debugOutput.text = "No Save Data found. Creating new data...";
            SaveGame();
        }
    }

    // what happens when we discover a weapon?
    public void DiscoverWeapon(GameObject weapon)
    {
        // get the weapon name
        string discoveringName = weapon.GetComponent<WeaponClass>().weaponName;
        // add that name to the discovered list in liveData
        liveData.DiscoveredWeapons.Add(discoveringName);
        // remove this from the undiscovered list in liveData
        liveData.UndiscoveredWeapons.Remove(discoveringName);
        // save the game
        SaveGame();
    }

    IEnumerator ClearDebug()
    {
        yield return new WaitForSecondsRealtime(4f);
        debugOutput.text = " ";
    }

}
