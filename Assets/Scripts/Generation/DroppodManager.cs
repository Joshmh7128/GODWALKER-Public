using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class DroppodManager : MonoBehaviour
{
    bool canLaunch = false;
    [SerializeField] Renderer greenZoneRenderer;
    [SerializeField] Material dimGreen;
    [SerializeField] Material brightGreen;
    [SerializeField] GenerationManager generationManager;

    private void Update()
    {
        if (canLaunch == true)
        {
            // make our green zone green
            greenZoneRenderer.material = brightGreen;
            // launch
            if (ReInput.players.GetPlayer(1).GetButtonDown("Space"))
            {
                generationManager.MapRegen();
                // launch the drop pod
                gameObject.GetComponent<Animator>().Play("Asteroid Hop");
            }

        }
        else
        {
            greenZoneRenderer.material = dimGreen;
        }
    }

    // when the player enters the green zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            canLaunch = true;
    }    

    // when the player enters the green zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canLaunch = false;
    }
}
