using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class DoorClass : MonoBehaviour
{
    /// this DoorClass is a new kind of doorclass seperate from the original
    /// this one will handle opening basic doors

    [SerializeField] Animator doorAnimator;
    Player player;
    Transform playerTransform;
    bool isOpen; // are we open?
    [SerializeField] float interactDistance;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
        playerTransform = UpgradeSingleton.Instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("ActionE") && (isOpen == false))
        {
            isOpen = true;
            doorAnimator.Play("QueueDoor");
        }

        if (!isOpen)
        {
           /* if (Vector3.Distance(playerTransform.position, transform.position) < interactDistance)
            {
                GameObject.Find("MusicManager").GetComponent<MusicController>().MusicMood(MusicController.musicMoods.tension);
            }*/
            
            if (Vector3.Distance(playerTransform.position, transform.position) > interactDistance)
            {
                GameObject.Find("MusicManager").GetComponent<MusicController>().MusicMood(MusicController.musicMoods.explore);
            }
        }
    }
}
