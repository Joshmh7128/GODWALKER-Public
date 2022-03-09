using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCannister : MonoBehaviour
{
    /// 
    /// script manages the breakable cannister object
    /// 

    // main variables
    [SerializeField] float dropAmount; // how much of our drop should we drop?
    [SerializeField] GameObject dropObject, breakParticle; // what object are we dropping?

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            // run our destruction function
            CustomDestroy();
        }
    }

    private void CustomDestroy()
    {
        // drop our stuff
        for (float i = dropAmount; i > 0; i--)
        {
            Instantiate(dropObject, transform.position, Quaternion.identity, null);
        }
        // spawn our break particle
        Instantiate(breakParticle, transform.position, Quaternion.identity, null);
        // shake the players screen
        Camera.main.GetComponent<CameraScript>().SnapScreenShake(1f);
        // destroy ourselves
        Destroy(gameObject);
    }
}
