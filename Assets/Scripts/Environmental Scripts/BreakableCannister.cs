using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] bool doesRecharge; // do we recharge?
    [SerializeField] float rechargeTimeSeconds, rechargeTimeRemaining; // how long do we recharge in seconds?
    [SerializeField] Image rechargeRing; // our recharge ring
    [SerializeField] GameObject cannisterParent; // the parent of our cannister object

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
                    UpgradeSingleton.Instance.player.AddResource(EnemyClass.dropTypes.HP, (int)dropAmount);
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
        // Run our collected function ourselves
        Collected();
    }

    private void Collected()
    {
        // do we recharge?
        if (!doesRecharge)
        {
            // break this object
            Destroy(gameObject);
        } else if (doesRecharge) // if we do recharge
        {
            // run our start Recharge function
            StartRecharge();
            // disable our cannister
            ToggleCannister(false);
        }
    }

    // cannister on/off
    void ToggleCannister(bool act)
    {
        if (act == true)
        {
            // make the cannister active
            cannisterParent.SetActive(true);
            // make the recharge image inactive
            rechargeRing.enabled = false;
        }

        if (act == false)
        {
            // make the cannister inactive
            cannisterParent.SetActive(false);
            // make the recharge image inactive
            rechargeRing.enabled = true;
        }
    }

    void StartRecharge()
    {
        // set our time
        rechargeTimeRemaining = rechargeTimeSeconds;
        // start our coroutine
        StartCoroutine(RechargeCountdown());
    }

    IEnumerator RechargeCountdown()
    {
        yield return new WaitForSeconds(1f);
        // on start, check if we have hit 0 on the recharge
        if (rechargeTimeRemaining <= 0)
        { // enable our object
            ToggleCannister(true);
        }

        if (rechargeTimeRemaining > 0)
        {
            // countdown
            rechargeTimeRemaining--;
            // set our image radial change
            rechargeRing.fillAmount = rechargeTimeSeconds - (rechargeTimeRemaining / rechargeTimeSeconds);
            // continue counting
            StartCoroutine(RechargeCountdown());
        }

    }

}
