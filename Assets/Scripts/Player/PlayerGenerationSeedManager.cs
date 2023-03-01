using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGenerationSeedManager : MonoBehaviour
{
    /// this script is used to generate a seed for the generation
    /// it is on the player because we don't want to lose this information on load
    /// 

    public string generationSeed; // our final generation seed
    [SerializeField] TextMeshProUGUI seedDisplay;
    public static PlayerGenerationSeedManager instance; // the instance of ourself

    public List<int> seedSetRanges; /// each index is the maximum number of rooms per area.
                                    /// For example, if area 0 only has 3 rooms, then seed set range index 0 is 3
                                    /// If area 1 has 5 rooms, then index 1 is set to 5
    public List<int> areaRoomAmount; // this designates how many rooms there are per area

    public int currentCombatPos, currentRunPos, debugPos = 3; // our current position in the seed, representing what will come next

    public bool shuffleAreas; // are we shuffling the areas

    public string nextRoom; // the public string representing the next room we want to go to

    // a public list of rooms we can go to for gamma generation
    public List<string> roomNames = new List<string>(); // set in inspector
    List<string> storedRoomNames = new List<string>(); // store our room names for when we reset the run

    public void Awake()
    {
        // setup instance
        instance = this;

        // deep copy room names
        foreach (string name in roomNames)
        {
            storedRoomNames.Add(name);
        }

    }

    // this is to setup our rooms in a progressive order, so that the player encounters maps and elements as they move through the run in a loose order
    public void BuildRoomNames()
    {
        // get all rooms by area
        List<string> a1 = new List<string>();
        List<string> a2 = new List<string>();

        foreach (string name in roomNames)
        {
            if (name.Contains("a1"))
                a1.Add(name);

            if (name.Contains("a2"))
                a2.Add(name);
        }

        // shuffle each list
        a1.Shuffle();
        a2.Shuffle();
        
        // progressively add
        roomNames.Clear();
        
        // build the room name list
        foreach(string name in a1) roomNames.Add(name); 
        foreach(string name in a2) roomNames.Add(name); 

    }

    // run this whenever want to start over
    public void ResetRun()
    {
        Debug.Log("Resetting run seed");
        // reset our run positions
        currentCombatPos = 0;
        currentRunPos = 0;
        debugPos = 0;

        // clear room names
        roomNames.Clear();

        // reset our room names
        foreach (string name in storedRoomNames)
        {
            roomNames.Add(name);
        }
    }
}
