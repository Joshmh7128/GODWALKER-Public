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

    [SerializeField] List<int> numOutput = new List<int>(); // our seed as a list
    [SerializeField] List<char> charDesignations = new List<char>(); // the designations of our character sets for area definitions
    [SerializeField] List<char> charOutput = new List<char>(); // the output of our designations to be used in the seed set

    public int currentPos; // our current position in the seed, representing what will come next
    [SerializeField] string nextPrefix; // the prefix we're using before our letter number designation

    public bool shuffleAreas; // are we shuffling the areas

    public string nextRoom; // the public string representing the next room we want to go to

    public void Awake()
    {
        instance = this;
    }

    // the construction of our seed
    public void BuildSeed()
    {
        // clear everything before generation
        generationSeed = "";
        numOutput.Clear();
        charOutput.Clear();
        currentPos = 0;

        // go through each seed set range and shuffle the number into a list
        for (int i = 0; i < areaRoomAmount.Count; i++)
        {
            // create a new list of ints
            List<int> localSeed = new List<int>();
            // add amounts from the range
            for (int j = 0; j < seedSetRanges[i]; j++)
            {
                localSeed.Add(j);
            }

            // then shuffle that list
            localSeed.Shuffle();

            // then add that list to our seed as list, only up to the area room amount 
            for (int k = 0; k < areaRoomAmount[i]; k++)
            {
                numOutput.Add(localSeed[k]);
            }
        }


        // now that we have our seed, we're going to turn it into a string
        string seedString = "";
        foreach (int num in numOutput)
        {
            seedString = seedString + num.ToString();
        }

        // then build out character designations
        for (int i = 0; i < areaRoomAmount.Count; i++)
        {
            // run a for loop and add one letter to our char output up to the seed range, so we can properly generate our next room based on position
            for (int c = 0; c < areaRoomAmount[i]; c++)
            {
                // add the range amount of the i character
                charOutput.Add(charDesignations[i]);
            }
        }

        // output our seed for debug purposes
        for (int i = 0; i < charOutput.Count; i++)
        {
            // are we shuffling the letters?
            if (shuffleAreas)
                charOutput.Shuffle();

            // add the letter, then add the number
            generationSeed = generationSeed + charOutput[i].ToString();
            generationSeed = generationSeed + numOutput[i].ToString();
        }

    }

    // based on which room we are currently in, which room will we go to?
    void ProcessCurrentPos()
    {
        // our next room will be prefix, current position output letter, current position seed number
        try
        {
            nextRoom = nextPrefix + charOutput[currentPos] + numOutput[currentPos];
            seedDisplay.text = generationSeed;
        }
         catch { }
    }

    private void Start()
    {
        BuildSeed();
    }

    private void FixedUpdate()
    {
        ProcessCurrentPos();
    }


}
