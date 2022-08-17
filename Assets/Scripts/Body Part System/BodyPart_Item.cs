using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart_Item : MonoBehaviour
{
    // script exists to be an item containing a part that the player can pickup

    public GameObject bodyPart; // the bodypart that this item contains

    // on start, instantiate our bodypart at the center of our object
    private void Start()
    {
        GameObject part = Instantiate(bodyPart, transform);
        part.transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Player")
        {

        }
    }
}
