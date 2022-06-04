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
    [SerializeField] Transform[] targetLocations;
    [SerializeField] int teleportCount;
    // our current target
    [SerializeField] Transform targetLocation;
    // our movement speed
    [SerializeField] float movementSpeed;
    // look targeting
    [SerializeField] Transform targeter;
    // visual fx
    [SerializeField] GameObject teleportParticle; 

    // start runs when this script is loaded
    void Start()
    {
        // FOR DEBUG LAUNCHING
        StartCoroutine(PhaseOne());
    }

    // update runs every frame
    void Update()
    {
        // run our movement
        Movement();
        // look at the player
        LookTargeter();
        // check our phase
        CheckPhase();

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

    void LookTargeter()
    {
        // move our targeter
        targeter.position = new Vector3(UpgradeSingleton.Instance.player.transform.position.x, transform.position.y, UpgradeSingleton.Instance.player.transform.position.z);
        // look at our targeter
        transform.LookAt(targeter.position);
    }

    // run our movement
    void Movement()
    {
        // if we are strafing, move towards our target location
        if (movementMode == movementModes.strafe)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, movementSpeed*Time.deltaTime);
        }        
        
        // if we are teleporting, blip towards our target location
        if (movementMode == movementModes.teleport)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, movementSpeed*Time.deltaTime*100);
        }
    }

    // call when we want to teleport
    void Blip()
    {
        // move through the list
        if (teleportCount < targetLocations.Length)
        {
            // move our body to a random spot
            targetLocation = targetLocations[Random.Range(0, targetLocations.Length)];
            movementMode = movementModes.teleport;
            teleportCount++;
        } else if (teleportCount >= targetLocations.Length)
        {
            // reset our teleport count
            teleportCount = 0;
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
 
    }

    void CheckPhase()
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
        Blip();
        yield return new WaitForSeconds(1f);
        // strafe for 4 seconds
        movementMode = movementModes.strafe;
        targetLocation = targetLocations[Random.Range(0, targetLocations.Length)];
        yield return new WaitForSeconds(4f);
        // teleport
        Blip();
        yield return new WaitForSeconds(1f);
        // strafe for 4 seconds
        movementMode = movementModes.strafe;
        targetLocation = targetLocations[Random.Range(0, targetLocations.Length)];
        yield return new WaitForSeconds(4f);
        // teleport
        Blip();
        yield return new WaitForSeconds(1f);
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
