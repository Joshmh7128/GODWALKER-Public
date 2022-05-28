using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BossBluestoneScript : EnemyClass
{
    /// <summary>
    /// this is to manage our first boss
    /// boss will have multiple phases according to HP amount
    /// boss will either teleport or strafe to the current target location
    /// </summary>

    // our states for our state machine
    enum states { 
        OPENER, // our opener
        P1A, // Spiral bullets, Teleport
        P1B, // Homing bullets while strafing, Teleport
        P1C, // Physics bullets while strafing, Teleport
        P2A, // Bursts of half-walls of bullets alternating in cardinal and dia-cardinal directions, Teleport
        P2B, // Homing and bouncing bullets, Teleport
        P2C, // Busts of linked bullets while strafing, Teleport
        P3A, // Bursts of full walls of bullets alternating in cardinal and dia-cardinal directions, Teleport
        P3B, // Homing and breakable bullets while strafing, Teleport
        P3C, // Bursts of Linked and homing while strafing, Teleport
    }
    // what is our current local state? cycle from 1 to 3 based on which attack pattern we did last
    int localState = 1;

    // our movement modes
    enum movementModes { strafe, teleport }
    movementModes movementMode;

    // our transform targets
    [SerializeField] Transform[] targetLocations, targetLocationsRandomized; int teleportCount;
    // our current target
    [SerializeField] Transform targetLocation;
    // our movement speed
    [SerializeField] float movementSpeed; 

    // start runs when this script is loaded
    void Start()
    {
        // setup our random locations as a list
        targetLocationsRandomized = targetLocations;
    }

    // update runs every frame
    void Update()
    {

    }

    // run our movement
    void Movement()
    {
        // if we are strafing, move towards our target location
        if (movementMode == movementModes.strafe)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, movementSpeed*Time.deltaTime); ;
        }
    }

    // call when we want to teleport
    void Teleport()
    {
        // move through the list
        if (teleportCount < targetLocations.Length)
        {
            // move our body to a random spot
            transform.position = targetLocationsRandomized[teleportCount].position;
            teleportCount++;
        }

        // if we are equal to or more than our length, randomize the list and set teleport count to 0
        if (teleportCount >= targetLocations.Length)
        {
            // reset our teleport count
            teleportCount = 0;

            // randomize our array
            for (int i = 0; i < targetLocations.Length; i++)
            {
                int j = Random.Range(0, targetLocations.Length);
                // if our desired location is already in the list, randomize until we find a position that is not
                while (targetLocationsRandomized.Contains<Transform>(targetLocations[j]))
                {
                    // test another one
                    j = Random.Range(0, targetLocations.Length);
                }

                // once we find one that is not in the array already, add it to the randomized list
                targetLocationsRandomized[i] = targetLocations[j];
            }
        }
    }

    public override void OnDeath()
    {

    }

    public override void Activate()
    {

    }   
    
    public override void Attack()
    {
        // check our HP
        if (HP >= maxHP * 0.66)
        {
            // check our local state
        }
        else if (HP < maxHP * 0.66 && HP >= maxHP * 0.33)
        {

        }
        else
        {

        }
    }

    public override void TakeDamage(int dmg)
    {

    }
}
