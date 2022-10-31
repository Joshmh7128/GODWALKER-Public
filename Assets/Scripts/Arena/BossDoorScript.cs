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
                fearCards.Remove(card);
                doorMesh.SetActive(false);

                // then kill all the cards
                foreach (var card2 in fearCards)
                {
                    Destroy(card2.gameObject);
                }

                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        doorMesh.SetActive(true);
    }

}

