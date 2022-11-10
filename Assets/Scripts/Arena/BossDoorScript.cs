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
                gameObject.SetActive(false);
                foreach (FearCard card2 in fearCards)
                {
                    try { card2.gameObject.SetActive(false); } catch { break; } finally { Debug.Log("Card inactive, loop broken"); }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        doorMesh.SetActive(true);
    }

}

