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
            if (GameObject.Find("MusicManager"))
            musicController = GameObject.Find("MusicManager").GetComponent<MusicController>(); 
        }
    }

    public void MusicRequest(MusicController.musicMoods mood)
    {
        if (musicController != null)
        {
            musicController.MusicMood(mood);
        }
    }
}
