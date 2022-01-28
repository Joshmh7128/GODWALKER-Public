using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    /// this script will handle our music and the current mood
    /// 

    [SerializeField] AudioSource musicSource; // we will have one audio source for our music at all times
    [SerializeField] AudioClip[] track001Pieces; // all the pieces of track001

    // our different music moods
    public enum musicMoods
    {
        explore,
        tension,
        doorQueue,
        battle
    }

    private void Start()
    {
        // set our music to the exploring mood at the start
        MusicMood(musicMoods.explore);
    }

    public void MusicMood(musicMoods mood)
    {
        // play the according track per request
        if (mood == musicMoods.explore)
        { musicSource.clip = track001Pieces[0]; }

        if (mood == musicMoods.tension)
        { musicSource.clip = track001Pieces[1]; }

        if (mood == musicMoods.doorQueue)
        { musicSource.clip = track001Pieces[2]; }

        if (mood == musicMoods.battle)
        { musicSource.clip = track001Pieces[3]; }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}
