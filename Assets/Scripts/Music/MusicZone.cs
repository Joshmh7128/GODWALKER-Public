using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{   
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.outro);
        }
    }
}
