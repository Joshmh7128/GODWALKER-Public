using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMusicManager : MonoBehaviour
{    
    ///<summary>
     /// This script plays adaptive music for us. 
     /// It plays an intro, combat, outro, and explore 
     /// Those enum choices are then activated at the end of a measure
     /// </summary>
     /// 

    // the length of one block (for Dynamic Track Test 01 a block is 4 measures at 156 bpm, or 6.15384 seconds)
    // [SerializeField] float blockLength = 6.15384f;

    public enum MusicMoods { intro, combat, outro, explore, godwalking }
    public MusicMoods currentMood;
    public MusicMoods desiredMood;

    // our music tracks
    [SerializeField] AudioClip intro, combatA, combatB, outro, explore, godwalking;

    // our stings
    [SerializeField] AudioClip combatCompleteSting; 

    [SerializeField] AudioSource musicSource, stingSource; // plays our sounds

    // setup our instance
    public static SimpleMusicManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (musicSource == null)
        {
            try { musicSource = GetComponent<AudioSource>(); }
            catch { Debug.LogError("Music manager has no Audio Source, you may need to assign it in the inspector"); }

        }

        PlaySong(MusicMoods.explore);
    }

    private void FixedUpdate()
    {
        if (currentMood != desiredMood)
        {
            PlaySong(desiredMood);
        }
    }

    public void PlaySong(MusicMoods mood)
    {
        Debug.Log("playing song " + mood);
        // set our desired mood in case it is not already set
        desiredMood = mood;

        // intro
        if (mood == MusicMoods.intro)
        {
            musicSource.clip = intro;
            musicSource.Play();
        }
        
        if (mood == MusicMoods.combat)
        {
            int i = Random.Range(0, 2);
            if (i == 0) musicSource.clip = combatA;
            if (i == 1) musicSource.clip = combatB;
            musicSource.Play();
        }

        // exploring
        if (mood == MusicMoods.explore)
        {
            musicSource.clip = explore;
            musicSource.Play();
        }

        if (mood == MusicMoods.outro)
        {
            // play the outro
            musicSource.clip = outro;
            musicSource.Play();
        }

        if (mood == MusicMoods.godwalking)
        {
            // play the outro
            musicSource.clip = godwalking;
            musicSource.Play();
        }
        
        // then set our current mood
        currentMood = mood;
    }

    public void RequestSting()
    {
        stingSource.clip = combatCompleteSting;
        stingSource.Play();
    }
}
