using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveTrackHandler : MonoBehaviour
{
    ///<summary>
    /// This script plays adaptive music for us. 
    /// We choose which instruments we want to play by changing the enum for each instrument
    /// Those enum choices are then activated at the end of a measure
    /// </summary>
    /// 

    // the length of one block (for Dynamic Track Test 01 a block is 4 measures at 156 bpm, or 6.15384 seconds)
    [SerializeField] float blockLength = 6.15384f;
    [SerializeField] AudioClip testLength;
    // use this to make enums
    enum TrackTypes { variant001, variant002, variant003, Count }
    public enum MusicMoods { explore, combat}

    // our instruments
    [SerializeField] List<TrackTypes> trackTypes = new List<TrackTypes>(); // make sure to set in editor!
    [SerializeField] List<Transform> trackParents = new List<Transform>(); // the transform parents that hold our instrument objects

    public Transform musicMoodExplore, musicMoodCombat;

    // setup our instance
    public static AdaptiveTrackHandler instance;
    private void Awake()
    {
        instance = this;
    }

    // start runs at the start of the game
    private void Start()
    {
        if (testLength)
        {
            blockLength = testLength.length;
        }

        // setup tracks
        // DebugRandomize();

        // start our music counting
        StartCoroutine(BlockCounter());
    }

    // use this to choose a mood instantly
    public void ChooseMood(MusicMoods mood)
    {
        MuteAll();
        Debug.Log("choosing mood");
        // clean our trackParents
        trackParents.Clear();

        if (mood == MusicMoods.explore)
        {
            SetupTracks(musicMoodExplore);
        }

        if (mood == MusicMoods.combat)
        {
            SetupTracks(musicMoodCombat);
        }
    }

    void SetupTracks(Transform moodParent)
    {
        foreach(Transform child in moodParent)
        {
            trackParents.Add(child);
        }
    }

    void MuteAll()
    {
        foreach (Transform parent in trackParents)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.GetComponent<AudioSource>().mute = true;
            }
        }
    }


    // our block counter
    private IEnumerator BlockCounter()
    {        
        // replay
        Replay();
        // randomize our selection
        // wait
        yield return new WaitForSecondsRealtime(blockLength);
        // then loop
        StartCoroutine(BlockCounter());
    }

    // where we trigger our track changes
    public void TriggerTrackChange(int maxTrack)
    {
        // change one of our tracks

        // Debug.Log(trackTypes[Random.Range(0, maxTrack)] = (TrackTypes)Random.Range(0, (float)TrackTypes.Count));

        DebugRandomize();
    }

    // randomize the track for testing
    private void DebugRandomize()
    {
        // loop through and randomize the tracks
        for (int i = 0; i < trackTypes.Count; i++)
        {
            trackTypes[i] = (TrackTypes)Random.Range(0, (int)TrackTypes.Count);
        }
    }

    // replay all tracks
    private void Replay()
    {
        // go through each parent
        for (int i = 0; i < trackParents.Count; i++)
        {
            // set all the children to inactive
            foreach (Transform child in trackParents[i])
            {
                child.gameObject.GetComponent<AudioSource>().mute = true;
            }
        }

        // activate the correlating children
        for (int j = 0; j < trackParents.Count; j++)
        {
            // for the parent activate the child in their correlating element of the track types list
            trackParents[j].GetChild((int)trackTypes[j]).GetComponent<AudioSource>().mute = false;
        }
    }

}
