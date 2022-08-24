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
    [SerializeField] float blockLength = 6.15384f;

    public enum MusicMoods { intro, combat, outro, explore }
    public MusicMoods musicMood;

    [SerializeField] List<AudioClip> queue = new List<AudioClip> ();

    [SerializeField] AudioClip intro, combat, outro, explore;

    int currentTrack; // which track are we on?

    [SerializeField] AudioSource musicSource; // plays music

    private void Start()
    {
        StartCoroutine(BlockCounter());
    }

    // our block counter
    private IEnumerator BlockCounter()
    {
        UpdateQueue();
        // PlayNext
        PlayNext();
        // wait
        yield return new WaitForSecondsRealtime(blockLength);
        // then loop
        StartCoroutine(BlockCounter());
    }

    // update our queue based on our moods
    void UpdateQueue()
    {
        // if our current mood is intro, add an introto the queue
        if (musicMood == MusicMoods.intro)
        {
            // stop our coroutine
            StopAllCoroutines();
            // add intro to the queue
            queue.Add(intro);
            // start our coroutine with our intro
            BlockCounter();
            // then set the mood to combat
            musicMood = MusicMoods.combat;
        }

        if (musicMood == MusicMoods.combat)
        {
            // add combat
            queue.Add(combat);
            // then set the mood to combat so that it plays again
            musicMood = MusicMoods.combat;
        }

        if (musicMood == MusicMoods.outro)
        {
            // add an outro to the queue
            queue.Add(outro);
            // then set us up to explore
            musicMood = MusicMoods.explore;
        }

        if (musicMood == MusicMoods.explore)
        {
            // add an outro to the queue
            queue.Add(explore);
            // then set us up to explore
            musicMood = MusicMoods.explore;
        }
    }

    void PlayNext()
    {
        musicSource.clip = queue[currentTrack];
        musicSource.Play();
        currentTrack++;
    }
}
