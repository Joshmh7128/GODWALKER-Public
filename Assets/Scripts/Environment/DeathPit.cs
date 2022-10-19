using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPit : MonoBehaviour
{
    /// this script is for whenever the player comes into contact with a death zone
    /// it runs a coroutine that in order requests a fade canvas to black, a player teleport, and a fade to white

    bool running;

    PlayerStatManager statManager;
    PlayerController controller;

    private void Start()
    {
        // get our instances
        statManager = PlayerStatManager.instance;
        controller = PlayerController.instance;
    }

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
