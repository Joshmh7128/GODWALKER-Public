using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : MonoBehaviour
{
    // the wizard flies to a point nearby the player and fires a fireball that picks up speed
    [SerializeField] Transform playerTransform; // our player's transform
    [SerializeField] float HP, maxHP; // our HP variables
    [SerializeField] Vector3 newPos;
    [SerializeField] float speed;
    private void Start()
    {
        // set our transform if we don't have one
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
    }

    private void Update()
    {
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // if we can see the player, start our moving and shooting coroutine

    }
}
