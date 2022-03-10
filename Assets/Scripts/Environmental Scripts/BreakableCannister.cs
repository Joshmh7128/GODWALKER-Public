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
    [SerializeField] bool dropsObject; // do we drop an object?
    [SerializeField] enum dropTypes { health, power, nanites}; // what kinds of things can we drop?
    [SerializeField] dropTypes dropType; // what is our droptype?

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
        if (dropsObject)
        {
            // drop our stuff
            for (float i = dropAmount; i > 0; i--)
            {
                Instantiate(dropObject, transform.position, Quaternion.identity, null);
            }
        } else
        {
            // depending on what we drop, give the player the selected values
            switch (dropType)
            {
                case dropTypes.health:
                    UpgradeSingleton.Instance.player.AddHP((int)dropAmount);
                    break;

                case dropTypes.nanites:
                    break;

                case dropTypes.power:
                    UpgradeSingleton.Instance.player.AddResource(EnemyClass.dropTypes.power, dropAmount);
                    break;
            }
        }
        // spawn our break particle
        Instantiate(breakParticle, transform.position, Quaternion.identity, null);
        // shake the players screen
        Camera.main.GetComponent<CameraScript>().SnapScreenShake(1f);
        // destroy ourselves
        Destroy(gameObject);
    }
}
