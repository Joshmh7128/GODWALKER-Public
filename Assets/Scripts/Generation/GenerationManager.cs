using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GenerationManager : MonoBehaviour
{
    /// <summary>
    /// This is generation version 9 and it'll handle which scenes are loaded into the map
    /// We're going to do a pretty simple line of thinking here
    /// 
    /// We're going to have a layout which has a list of room lists to fill each chunk of the environment
    /// This script will pull rooms from those lists and choose one for each slot in our area
    /// </summary>
    /// 

    // our list of layouts
    [SerializeField] List<LayoutList> layouts; // out list of layouts

    // on enable triggers when the obejct becomes active in the hierarchy
    private void OnEnable()
    {
        GenerateMap(); // generate the map when we start the game
    }

    // our function for loading in every single room
    void GenerateMap()
    {
        // choose a layout
        int i = Random.Range(0, layouts.Count); // +1 since it is a float
        LayoutList layoutList = layouts[i]; // this is out layout
        // choose rooms from that layout
        foreach (RoomSceneList roomSceneList in layoutList.roomSceneLists)
        {
            // get a random room and load that room in additively
            string scene = roomSceneList.chunks[Random.Range(0, roomSceneList.chunks.Count)];
            // spawn in that scene
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

}
