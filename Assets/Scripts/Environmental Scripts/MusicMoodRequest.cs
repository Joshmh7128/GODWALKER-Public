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

    // get our door parent and ask it to play its music
    public void DoorParentMusicRequest()
    {
        if (transform.parent != null)
        {
            transform.parent.GetComponent<DoorClass>().DoorMusicTrigger();
        }
    }
}
