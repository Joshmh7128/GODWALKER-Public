using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFlyerMinion : EnemyClass
{
    ///
    /// this script will manage our basic flying enemy
    /// We want this enemy to move around relatively slowly in the air,
    /// and fire more frequently than our basic tread minion
    /// 
    [SerializeField] Vector3 targetPosition; // where we are trying to move to
    [SerializeField] float fireRate; // how quickly we fire shots (deterministic animation speed)
    [SerializeField] PlayerController playerController; // our player controller
    [SerializeField] float movementSpeed, hMovementRadius, vMovementRadius, maxMoveDelta/*how close we need to go to our target pos*/, enemyRadius; // how much can we move around on the horizontal and vertical axes
    bool pathBlocked; // is our path blocked?
    [SerializeField] float flightWaitTime; // how long do we wait in between making movements?

    private void Start()
    {
        // find our player controller on start
        playerController = FindObjectOfType<PlayerController>();

        // set our maxMoveDelta
        maxMoveDelta = movementSpeed * 2;
    }

    private void FixedUpdate()
    {
        // if we are active in combat
        if (isActive)
        {
            // always move towards our target position if we are far away from it and our path is not blocked
            if (Vector3.Distance(transform.position, targetPosition) > maxMoveDelta && !pathBlocked)
            {
                // apply movement
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }

            Ray movementSphereCast = new Ray(); 
            movementSphereCast.origin = transform.position; movementSphereCast.direction = transform.position - targetPosition; 
            // perform a spherecast towards our target position from our transport position
            if (Physics.SphereCast(movementSphereCast, enemyRadius, maxMoveDelta, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                pathBlocked = true;
            } else if (!Physics.SphereCast(movementSphereCast, enemyRadius, maxMoveDelta, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                pathBlocked = false;
            }
        }
    }

    public override void Activate()
    {
        // set isactive to true
        isActive = true;
        // start our movement coroutine
        StartCoroutine(MovementPattern());
    }

    // this will control our movement patterns
    IEnumerator MovementPattern()
    {
        // choose a position around the player on the X and Z axes
        targetPosition = playerController.transform.position + new Vector3(Random.Range(-hMovementRadius, hMovementRadius), Random.Range(-vMovementRadius, vMovementRadius), Random.Range(-hMovementRadius, hMovementRadius));
        // wait before moving again
        yield return new WaitForSeconds(flightWaitTime);
    }

    public override void TakeDamage(int dmg)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeath()
    {
        throw new System.NotImplementedException();
    }
}
