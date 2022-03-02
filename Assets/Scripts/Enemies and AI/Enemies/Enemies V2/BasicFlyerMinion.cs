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
    PlayerController playerController; // our player controller
    [SerializeField] float movementSpeed, hMovementRadius, vMovementRadius, /*how close we need to go to our target pos*/ enemyRadius; // how much can we move around on the horizontal and vertical axes
    float maxMoveDelta; 
    bool pathBlocked; // is our path blocked?
    [SerializeField] float flightWaitTime; // how long do we wait in between making movements?
    [SerializeField] Transform bodyContainer, mainContainer, rotationalContainer; // our transform containers
    [SerializeField] Animator damageAnimator, mainAnimator;
    [SerializeField] float rotationalKick; // how much do we rotational kick when we get hit?
    [SerializeField] GameObject deathParticle, bulletPrefab, muzzleFlashParticle; // our death particle system, our projectile
    [SerializeField] Transform rightShotPos, leftShotPos; // our left and right shot positions
    Vector3 shotPos;
    bool isRight = true;
    [SerializeField] bool changesSides = false; // are we shooting from the right shot spot? do our sides change

    private void Start()
    {
        // find our player controller on start
        playerController = UpgradeSingleton.Instance.player;

        // set our maxMoveDelta
        maxMoveDelta = movementSpeed/30;

        // activate for development purposes
        Activate();
    }

    private void Update()
    {
        if (HP <= 0)
        {
            if (combatZone) { combatZone.OnDeath(); }
            OnDeath(); 
        }
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
            movementSphereCast.origin = transform.position; movementSphereCast.direction = targetPosition - transform.position ; 
            // perform a spherecast towards our target position from our transport position
            if (Physics.SphereCast(movementSphereCast, enemyRadius, maxMoveDelta, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                pathBlocked = true;
            } else if (!Physics.SphereCast(movementSphereCast, enemyRadius, maxMoveDelta, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                pathBlocked = false;
            }
        }

        // have our body container look at the player
        bodyContainer.LookAt(playerController.transform.position, Vector3.up);

        // lerp our rotation container back to it's original rotation after it gets hit
        rotationalContainer.rotation = Quaternion.Lerp(rotationalContainer.rotation, bodyContainer.rotation, 5 * Time.deltaTime);
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

        int dec = Random.Range(0, 2);
        // choose a position around the player on the X and Z axes
        if (dec == 0)
        {
            targetPosition = playerController.gameObject.GetComponent<Transform>().position + new Vector3(Random.Range(-hMovementRadius, hMovementRadius), Random.Range(transform.position.y + -vMovementRadius, transform.position.y + vMovementRadius), Random.Range(-hMovementRadius, hMovementRadius));
        } else if (dec != 0)
        {
            targetPosition = transform.position + new Vector3(Random.Range(-hMovementRadius, hMovementRadius), 0f, Random.Range(-hMovementRadius, hMovementRadius));
        }

        // wait before moving again
        yield return new WaitForSeconds(flightWaitTime);
        // start again
        StartCoroutine(MovementPattern());
    }

    public override void TakeDamage(int dmg)
    {
        // Debug.Log("Damage");
        // reduce our HP
        HP -= dmg;
        // do our damage flicker
        mainAnimator.Play("Damage");
        // rotate our rotational body 
        RandomRotationKick(rotationalKick);
    }

    public override void OnDeath()
    {
        // then blow up
        Instantiate(deathParticle, transform.position, Quaternion.identity, null);
        // then destroy ourselves
        Destroy(gameObject);
    }

    public void RandomRotationKick(float kickAmount)
    {
        // rotate our rotational body 
        rotationalContainer.rotation = Quaternion.Euler(new Vector3(bodyContainer.rotation.eulerAngles.x + Random.Range(-kickAmount / 10, kickAmount / 10), bodyContainer.rotation.eulerAngles.y + Random.Range(-kickAmount, kickAmount), bodyContainer.rotation.eulerAngles.z + Random.Range(-kickAmount / 10, kickAmount / 10)));
    }

    public override void Attack()
    {
        

        mainAnimator.speed = Random.Range(0.75f, 1.25f);

        // determine our shot position
        if (isRight) { shotPos = rightShotPos.position; }
        if (!isRight) { shotPos = leftShotPos.position; }

        // fire our shot
        Instantiate(bulletPrefab, shotPos, bodyContainer.rotation);
        Instantiate(muzzleFlashParticle, shotPos, bodyContainer.rotation);
        // flip which side we are on
        if (changesSides)
        isRight = !isRight;
    }
}
