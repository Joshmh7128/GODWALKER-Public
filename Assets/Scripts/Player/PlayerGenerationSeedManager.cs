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
    public static PlayerGenerationSeedManager instance; // the instance of ourself

    public int currentCombatPos, currentRunPos; // our current position in the seed, representing what will come next

    public bool shuffleAreas; // are we shuffling the areas

    public string nextRoom; // the public string representing the next room we want to go to

    // a public list of rooms we can go to for gamma generation
    public List<string> roomNames = new List<string>(); // set in inspector
    [SerializeField] List<string> rewardNames = new List<string>(); // the names of our shops
    [SerializeField] List<string> storedRoomNames = new List<string>(); // store our room names for when we reset the run
    [SerializeField] int shopFrequency; // how often do we see shops throughout the run?
    // our element selections
    public enum ElementBiases
    {
        none, 
        partialEnergy,      // 50% of enemies have energy shields
        partialExplosive,   // 50% of enemies have explosive shields
        partialMixed,       // 25% energy, 25% explosive
        allEnergy,          // all enemies have energy shields
        allExplosive,        // all enemies have explosive shields
        freeGun, specialGun, shop, finish 
    }

    // our list of element preferences for each room in the run
    public List<ElementBiases> elementBiases = new List<ElementBiases>();

    public bool roomListLoaded; // are our rooms loaded and ready to be pulled from?

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

    private void Start()
    {
        // construct our playlist
        BuildRoomNames();
    }

    // this is to setup our rooms in a progressive order, so that the player encounters maps and elements as they move through the run in a loose order
    public void BuildRoomNames()
    {
        roomNames.Clear();

        // get all rooms by area
        List<string> A1 = new List<string>();
        List<string> A2 = new List<string>();

        foreach (string name in storedRoomNames)
        {
            if (name.Contains("A1"))
                A1.Add(name);

            if (name.Contains("A2"))
                A2.Add(name);
        }
        
        // build the room name list
        A1.Shuffle(); A2.Shuffle();

        // add the names progressively
        foreach (string name in A1) roomNames.Add(name);
        foreach (string name in A2) roomNames.Add(name);

        // then add in our rewards every shopFrequency within the list, then add a shop at the end
        for (int i = shopFrequency; i < roomNames.Count; i += shopFrequency)
            roomNames.Insert(i, rewardNames[0]);
        roomNames.Insert(roomNames.Count / 2, rewardNames[1]);

        // at the end of the run add the finish
        roomNames.Add("Finish");

        // after the maps are selected, choose our elements
        AssignElementalBiases();

    }

    // function which decides our elemental biases
    public void AssignElementalBiases()
    {
        // make sure element list is the same length as room length
        foreach (string name in roomNames) elementBiases.Add(ElementBiases.none);

        // loop through our rooms by length and determine our place in the run. assign element accordingly.
        for (int i = 0; i < roomNames.Count; i++)
        {
            // check i's percentage position
            float x = ((float)i/roomNames.Count)*100;

            /// for 0% to 15% of the run use no elements
            if (x < 30)
            {
                elementBiases[i] = ElementBiases.none;
            }

            /// for 31% to 70% use partial elements
            if (x > 30 && x < 60)
            {
                int c = Random.Range(0, 2); // make a 33/33/33 rand
                if (c == 0) elementBiases[i] = ElementBiases.partialEnergy;
                if (c == 1) elementBiases[i] = ElementBiases.partialExplosive;
                if (c == 2) elementBiases[i] = ElementBiases.partialExplosive;
                //if (c == 2) elementBiases.Add(ElementBiases.partialMixed);
            }

            /// for 71% to 100% use 'all' elements
            if (x > 60)
            {
                int c = Random.Range(0, 3); // make a 50/50 rand
                if (c == 0) elementBiases[i] = ElementBiases.allEnergy;
                if (c == 1) elementBiases[i] = ElementBiases.allExplosive;
                if (c == 2) elementBiases[i] = ElementBiases.partialMixed;
                if (c == 3) elementBiases[i] = ElementBiases.partialMixed;
            }
        }

        try
        {
            // now go through and compare our elements to our room list, and override them based on specials
            for (int i = 0; i < roomNames.Count; i++)
            {
                // reassign rewards
                if (roomNames[i] == rewardNames[0])
                    elementBiases[i] = ElementBiases.freeGun;

                // reassign shops
                if (roomNames[i] == rewardNames[1])
                    elementBiases[i] = ElementBiases.shop;

                if (roomNames[i] == "Finish")
                    elementBiases[i] = ElementBiases.finish;
            }
        } catch
        {
            // this means the seed is not done generating, run the elemental assignments again
            AssignElementalBiases();
        }
    }

    // run this whenever want to start over
    public void ResetRun()
    {
        Debug.Log("Resetting run seed");
        // reset our run positions
        currentCombatPos = 0;
        currentRunPos = 0;
        // build the rooms
        BuildRoomNames();
    }
}
