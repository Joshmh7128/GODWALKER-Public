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

    public enum MusicMoods { intro, outro, explore }
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
        PlaySong(MusicMoods.explore);
    }

    public void PlaySong(MusicMoods mood)
    {
        // make sure we arent about to play another song
        StopAllCoroutines();

        // intro
        if (mood == MusicMoods.intro)
        {
            musicSource.clip = intro;
            musicSource.Play();
            // then delay our combat music
            StartCoroutine(DelayPlay(intro.length, combat));
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
            // delay play explore
            StartCoroutine(DelayPlay(outro.length-0.001f, explore));
        }
    }

    IEnumerator DelayPlay(float time, AudioClip clip)
    {
        yield return new WaitForSeconds(time);
        musicSource.clip = clip;
        musicSource.Play();
    }
    
}
