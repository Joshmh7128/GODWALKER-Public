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
    public enum states { 
        OPENER, // our opener
        PhaseOne,
        PhaseTwo,
        PhaseThree,
    }
    // our state
    public states state;

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

        // FOR DEBUG LAUNCHING
        StartCoroutine(PhaseOne());
    }

    // update runs every frame
    void Update()
    {
        // run our movement
        Movement();

        // check our phase every frame
        if (state == states.PhaseTwo)
        {
            // stop phase 1
            StopCoroutine(PhaseOne());
        }

        if (state == states.PhaseThree)
        {
            // stop phase 1
           //  StopCoroutine(PhaseTwo());
        }
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
        // check our HP and set our phase
        if (HP >= maxHP * 0.66)
        {
            state = states.PhaseOne;
        }
        else if (HP < maxHP * 0.66 && HP >= maxHP * 0.33)
        {
            state = states.PhaseTwo;
        }
        else
        {
            state = states.PhaseThree;
        }
    }

    // run through our three phase 1 animations
    IEnumerator PhaseOne()
    {
        // fire our shots

        // teleport
        Teleport();
        // strafe for 4 seconds
        movementMode = movementModes.strafe;
        targetLocation = targetLocationsRandomized[Random.Range(0,targetLocationsRandomized.Length)];
        yield return new WaitForSeconds(4f);
        // teleport
        Teleport();
        // strafe for 4 seconds
        movementMode = movementModes.strafe;
        targetLocation = targetLocationsRandomized[Random.Range(0, targetLocationsRandomized.Length)];
        yield return new WaitForSeconds(4f);
        // teleport
        Teleport();
        // if we can, repeat phase one
        if (state == states.PhaseOne)
        {
            StartCoroutine(PhaseOne());
        }
    }

    public override void TakeDamage(int dmg)
    {

    }
}
