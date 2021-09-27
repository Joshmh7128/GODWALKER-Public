using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSpot : MonoBehaviour
{
    public GameObject interactionMouse;
    public Transform cameraPos;

    // if we come in to contact with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().interactionCameraPos = cameraPos;
            other.gameObject.GetComponent<PlayerController>().interactionMouse = interactionMouse;
            other.gameObject.GetComponent<PlayerController>().cameraScript.canLook = false;
            other.gameObject.GetComponent<PlayerController>().canFire = false;
            interactionMouse.GetComponent<InteractionMouse>().canMove = true;
        }
    }    
    
    // if we come in to contact with the player
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().interactionCameraPos = cameraPos;
            other.gameObject.GetComponent<PlayerController>().interactionMouse = interactionMouse;
            other.gameObject.GetComponent<PlayerController>().cameraScript.canLook = true;
            other.gameObject.GetComponent<PlayerController>().canFire = true;
            interactionMouse.GetComponent<InteractionMouse>().canMove = false;
        }
    }
}
