using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerationSeedManager : MonoBehaviour
{
    /// this script is used to generate a seed for the generation
    /// it is on the player because we don't want to lose this information on load
    /// 

    public string generationSeed; // our final generation seed

    public static PlayerGenerationSeedManager instance; // the instance of ourself

    public List<int> seedSetRanges; /// each index is the maximum number of rooms per area.
                                    /// For example, if area 0 only has 3 rooms, then seed set range index 0 is 3
                                    /// If area 1 has 5 rooms, then index 1 is set to 5

    [SerializeField] List<int> numOutput = new List<int>(); // our seed as a list
    [SerializeField] List<char> charDesignations = new List<char>(); // the designations of our character sets for area definitions
    [SerializeField] List<char> charOutput = new List<char>(); // the output of our designations to be used in the seed set

    public int currentPos; // our current position in the seed, representing what will come next
    [SerializeField] string nextPrefix; // the prefix we're using before our letter number designation

    public string nextRoom; // the public string representing the next room we want to go to

    // the construction of our seed
    public void BuildSeed()
    {
        // go through each seed set range and shuffle the number into a list
        foreach (int seedRange in seedSetRanges)
        {
            // create a new list of ints
            List<int> localSeed = new List<int>();
            for (int i = 0; i < seedRange; i++)
            {
                localSeed.Add(i);
            }

            // then shuffle that list
            localSeed.Shuffle();

            // then add that list to our seed as list
            foreach(int seed in localSeed)
            {
                numOutput.Add(seed);
            }
        }


        // now that we have our seed, we're going to turn it into a string
        string seedString = "";
        foreach (int num in numOutput)
        {
            seedString = seedString + num.ToString();
        }

        // then build out character designations
        for (int i = 0; i < seedSetRanges.Count; i++)
        {
            // run a for loop and add one letter to our char output up to the seed range, so we can properly generate our next room based on position
            for (int c = 0; c < seedSetRanges[i]; c++)
            {
                // add the range amount of the i character
                charOutput.Add(charDesignations[i]);
            }
        }

        // output our seed for debug purposes
        for (int i = 0; i < charOutput.Count; i++)
        {
            // add the letter, then add the number
            generationSeed = generationSeed + charOutput[i];
            generationSeed = generationSeed + numOutput[i];
        }

    }

    // based on which room we are currently in, which room will we go to?
    void ProcessCurrentPos()
    {
        // our next room will be prefix, current position output letter, current position seed number
        nextRoom = nextPrefix + charOutput[currentPos] + numOutput[currentPos];
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
