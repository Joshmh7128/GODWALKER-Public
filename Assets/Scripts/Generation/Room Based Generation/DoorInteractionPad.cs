using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionPad : MonoBehaviour
{
    // this pad links to a door and triggers it's opening
    bool canOpen; // can we open the door?
    [SerializeField] bool specialDoor; // can we open the door?
    [SerializeField] DoorClass doorClass; // our door class

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
        if (canOpen && specialDoor)
        {
            if (GameObject.Find("Player").GetComponent<PlayerController>().gemAmount > 100)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    doorClass.OpenDoor();
                    GameObject.Find("Player").GetComponent<PlayerController>().gemAmount -= 100;
                }
            }
        }        
        
        if (canOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                doorClass.OpenDoor();
            }
        }
    }
}
