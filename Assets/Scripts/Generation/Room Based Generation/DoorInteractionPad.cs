using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class DoorInteractionPad : MonoBehaviour
{
    // this pad links to a door and triggers it's opening
    bool canOpen; // can we open the door?
    [SerializeField] bool specialDoorGem; // can we open the door?
    [SerializeField] bool specialDoorScrap; // can we open the door?
    [SerializeField] DoorClass doorClass; // our door class
    Player player; // our player

    private void Start()
    {
        // get our player
        player = ReInput.players.GetPlayer(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canOpen = false;
        }
    }

    private void Update()
    {
        if (canOpen && specialDoorGem)
        {
            if (GameObject.Find("Player").GetComponent<PlayerController>().gemAmount > 100)
            {
                if (player.GetButtonDown("Interaction"))
                {
                    doorClass.OpenDoor();
                    GameObject.Find("Player").GetComponent<PlayerController>().gemAmount -= 100;
                }
            }
        }

        if (canOpen && specialDoorScrap)
        {
            if (GameObject.Find("Player").GetComponent<PlayerController>().gemAmount > 10)
            {
                if (player.GetButtonDown("Interaction"))
                {
                    doorClass.OpenDoor();
                    GameObject.Find("Player").GetComponent<PlayerController>().gemAmount -= 10;
                }
            }
        }        
        
        
        if (canOpen && !specialDoorGem && !specialDoorScrap)
        {
            if (player.GetButtonDown("Interaction"))
            {
                doorClass.OpenDoor();
            }
        }
    }
}
