using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPit : MonoBehaviour
{
    /// this script is for whenever the player comes into contact with a death zone
    /// it runs a coroutine that in order requests a fade canvas to black, a player teleport, and a fade to white

    bool running;
    bool counting;
    float count, countMax = 5; // how long have we been counting?

    PlayerStatManager statManager;
    PlayerController controller;

    // our collider
    Collider boxCollider; // our collider

    private void Start()
    {
        // check to see what we have
        boxCollider.isTrigger = !statManager.lavaWalks; // make it the opposite. if we can walk on lava, this should not be a trigger

        // get our instances
        statManager = PlayerStatManager.instance;
        controller = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        // set up our collider properly
        boxCollider.isTrigger = !statManager.lavaWalks;

        // if we can count...
        if (counting)
        {
            count++;

            if (count > countMax * 60)
            {
                // set count to 0
                count = 0;
                // deal player damage
                statManager.TakeDamage(statManager.maxHealth * 0.1f);
            }

        }
    }

    // if player can lava walk
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            counting = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            counting = false;
            count = 0;
        }
    }

    // if player cannot lava walk
    private void OnTriggerEnter(Collider collision)
    {
        // if we collide with the player and our coroutine is not running
        if (collision.transform.tag == "Player" && !running)
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    // the coroutine 
    IEnumerator TeleportPlayer()
    {
        // trigger the fade to black
        statManager.fadeUITargetA = 1;
        // wait for that to complete
        yield return new WaitForSeconds(1f);
        // teleport player to the last place they were grounded at
        controller.Teleport(controller.lastGroundedPos);
        // take 5% damage
        statManager.TakeDamage(statManager.maxHealth*0.05f);
        // trigger the fade to white
        statManager.fadeUITargetA = 0;
    }


}
