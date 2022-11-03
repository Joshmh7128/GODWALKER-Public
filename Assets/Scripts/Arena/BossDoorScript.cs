using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoorScript : MonoBehaviour
{
    // this handles all of our boss doors
    [SerializeField] List<FearCard> fearCards;
    [SerializeField] GameObject doorMesh; 

    private void FixedUpdate()
    {
        // if we take any cards, open the door
        foreach (var card in fearCards)
        {
            
            if (card == null)
            {
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        doorMesh.SetActive(true);
    }

}

