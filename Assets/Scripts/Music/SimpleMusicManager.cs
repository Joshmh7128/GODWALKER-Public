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

    public enum MusicMoods { intro, combat, outro, explore }
    public MusicMoods musicMood;

    [SerializeField] AudioClip intro, combat, outro, explore;

    [SerializeField] AudioSource musicSource; // plays music

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

            PlaySong(MusicMoods.explore);
        }
    }

    public void PlaySong(MusicMoods mood)
    {
        // intro
        if (mood == MusicMoods.intro)
        {
            musicSource.clip = intro;
            musicSource.Play();
        }
        
        if (mood == MusicMoods.combat)
        {
            musicSource.clip = combat;
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
    }
}
