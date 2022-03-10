using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropScript : MonoBehaviour
{
    /// <summary>
    /// This script handles all normal drops from enemies
    /// Includes HP, Nanites, and Power. Specialized scripts will be
    /// written for other kinds of things.
    /// </summary>

    [SerializeField] float pickupDistance, moveSpeed, maxMoveSpeed, value; // how far away does the player have to be for us to be picked up?
    [SerializeField] GameObject playerObject; // player transform
    bool canMove = false;
    [SerializeField] EnemyClass.dropTypes dropType;

    private void Start()
    {
        // get the player's transform
        playerObject = UpgradeSingleton.Instance.player.gameObject;
    }

    private void FixedUpdate()
    {
        // check out pickup distance
        if (Vector3.Distance(playerObject.transform.position, transform.position) < pickupDistance)
        { canMove = true; }
    }

    private void Update()
    {
        // if we were ever within pickup distance of the player, speed up until we hit them
        if (canMove)
        {
            // speed 
            if (moveSpeed < maxMoveSpeed)
            {
                moveSpeed++;
            }
            // apply our movement
            transform.position = Vector3.MoveTowards(transform.position, playerObject.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            // trigger the resource pickup
            playerObject.GetComponent<PlayerController>().AddResource(dropType, value);
            // destroy this gameobject
            Destroy(gameObject);
        }
    }
}
