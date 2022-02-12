using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    /// 
    /// class exists to work with jump pads
    /// 

    [SerializeField] float jumpPower;
    [SerializeField] AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        // if our trigger comes into contact with the player
        if (other.transform.tag == "Player")
        {   // launch them
            UpgradeSingleton.Instance.player.JumpLaunch(jumpPower);
            // play our sound
            if (audioSource)
            audioSource.Play();
        }
    }
}
