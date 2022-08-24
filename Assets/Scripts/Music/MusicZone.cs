using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AdaptiveTrackHandler.MusicMoods targetMood;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            AdaptiveTrackHandler.instance.ChooseMood(targetMood);
        }
    }
}
