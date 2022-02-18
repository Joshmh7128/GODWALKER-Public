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
    [SerializeField] CombatZone nextCombatZone, pastCombatZone; // our associated combat zone to activate. our past combat zone to see if we can open

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
        playerTransform = UpgradeSingleton.Instance.player.transform;

        if (Vector3.Distance(playerTransform.position, transform.position) > interactDistance)
        {
            if (GameObject.Find("MusicManager"))
            GameObject.Find("MusicManager").GetComponent<MusicController>().MusicMood(MusicController.musicMoods.explore);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("ActionE") && (isOpen == false) && Vector3.Distance(playerTransform.position, transform.position) < interactDistance)
        {
            isOpen = true; // we're open
            doorAnimator.Play("QueueDoor"); // animate!
            nextCombatZone.ActivateZone(); // activate the zone
        }

        if (!isOpen)
        {
           /* if (Vector3.Distance(playerTransform.position, transform.position) < interactDistance)
            {
                GameObject.Find("MusicManager").GetComponent<MusicController>().MusicMood(MusicController.musicMoods.tension);
            }*/
            

        }
    }
}
