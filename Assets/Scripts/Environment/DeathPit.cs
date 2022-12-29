using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPit : MonoBehaviour
{
    /// this script is for whenever the player comes into contact with a death zone
    /// it runs a coroutine that in order requests a fade canvas to black, a player teleport, and a fade to white

    bool counting;
    [SerializeField] float count, countMax = 5; // how long have we been counting?
    [SerializeField] float damage; // how much damage do we deal to the player?
    PlayerStatManager statManager;

    private void Start()
    {
        statManager = PlayerStatManager.instance;
    }

    private void FixedUpdate()
    {
        // if we can count...
        if (counting)
        {
            Debug.Log("counting");
            count++;

            if (count > countMax * 60)
            {
                // set count to half of count max 8 60
                count = (countMax * 60) / 2;
                // deal player damage
                Debug.Log("damaging");
                statManager.TakeDamage(damage);
            }

        }
    }

    // on trigger enter start counting
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
            counting = true;
    }

    // on trigger exit stop counting
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
            counting = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
            counting = true;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
            counting = false;
    }

    // the coroutine 
    IEnumerator TeleportPlayer()
    {
        // trigger the fade to black
        statManager.fadeUITargetA = 1;
        // wait for that to complete
        yield return new WaitForSeconds(1f);
        // teleport player to the last place they were grounded at
        // controller.Teleport(controller.lastGroundedPos);
        // take 5% damage
        statManager.TakeDamage(statManager.maxHealth*0.05f);
        statManager.damageCooldown = 300; // make sure the player can't take damage the moment they respawn
        // trigger the fade to white
        statManager.fadeUITargetA = 0;
        yield return null;
    }


}
