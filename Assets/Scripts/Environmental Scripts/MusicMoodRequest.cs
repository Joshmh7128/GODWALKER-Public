using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMoodRequest : MonoBehaviour
{
    MusicController musicController;

    // Start is called before the first frame update
    void Start()
    {
        if (musicController == null)
        { 
            musicController = GameObject.Find("MusicManager").GetComponent<MusicController>(); 
        }
    }

    public void MusicRequest(MusicController.musicMoods mood)
    {
        musicController.MusicMood(mood);
    }
}
