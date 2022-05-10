using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlowerTurretMinion : EnemyClass
{
    /// the tread turret minion is a small enemy who fires shots at the player
    /// they move to random close points within a thick radius around the player when they are above 50% HP
    /// when they are below 50% HP they will move away from the player, and attempt to run from them
    /// when they shoot they fire three shots at the player, one direct, one slightly left and one slightly right

    // our health point variables are defined in the EnemyClass; HP and MaxHP are defined in the parent abstract class
    [SerializeField] private Transform playerTransform; // our player's transform
    [SerializeField] private float activationDistance, closeRadiusMin, closeRadiusMax, farRadiusMin, farRadiusMax; // our close and far radii
    private float x, y, z, rx, rz, headHeight; // our movement variables
    [SerializeField] float animationSpeedMin = 0.75f, animationSpeedMax = 1.25f;
    [SerializeField] float navMeshWaitMin = 0.5f, navMeshWaitMax = 1f;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] Transform headJoint, treadTransform, treadRaycastStart; // our head joint

    [SerializeField] GameObject deathParticle;  // our death particle
    [SerializeField] GameObject enableParticle;  // our death particle
    bool hasActivated = false; // have we activtated?
    [SerializeField] bool isBomb; // are we a bomb?

    // shooting stuff
    float fireRate; // how fast we fire
    [SerializeField] GameObject bulletPrefab;  // what we are firing
    [SerializeField] Transform[] shotPositions; // our shot positions

    private void Start()
    {
        // get our player transform
        if (playerTransform == null)
        { playerTransform = UpgradeSingleton.Instance.player.transform; }

        // get our nav mesh agent
        if (navMeshAgent == null)
        { navMeshAgent = GetComponent<NavMeshAgent>(); }
    }

    // our movement coroutine
    private IEnumerator NavMeshPositionTarget()
    {
        // if our HP is > 50% choose a position in our close radius, if < 50% choose in our far radius
        // top 50%

        // are we doing local or player based movement?
        int dec = Random.Range(0, 2);
        // calculate movement vectors
        if (HP > maxHP / 2f) { x = Random.Range(closeRadiusMin, closeRadiusMax); z = Random.Range(closeRadiusMin, closeRadiusMax); }
        // bottom 50%
        if (HP < maxHP / 2f)
        { x = Random.Range(farRadiusMin, farRadiusMax); z = Random.Range(farRadiusMin, farRadiusMax); }
        // shuffle those to be positive or negative within the radii
        rx = Random.Range(0, 2); rz = Random.Range(0, 2); if (rx > 0) { x *= -1; }
        if (rz > 0) { z *= -1; }
        // get the X and Z of that position and use the Y of our head so that we can walk up slopes (may need refining?)
        y = headHeight;
        if (dec == 0)
        {
            navMeshAgent.destination = playerTransform.position + new Vector3(x, y, z);
        }
        else if (dec != 0)
        {
            navMeshAgent.destination = playerTransform.position + new Vector3(x * 2, y, z * 2);
        }

        // wait 1 to 3 seconds
        yield return new WaitForSeconds(Random.Range(navMeshWaitMin, navMeshWaitMax));
        // repeat this cycle
        StartCoroutine(NavMeshPositionTarget());
    }

    // update runs once per calculated frame
    private void Update()
    {
        // death condition
        if (HP <= 0)
        {
            OnDeath(); // destroy this enemy through our death function
        }

        // rotate our headjoint to look at nothing
        headJoint.transform.LookAt(new Vector3(0, headJoint.transform.position.y, 0));
    }

    // the fixed update runs 60 times per second
    private void FixedUpdate()
    {
        // if we can see the player or if our player is within reaction range, activate our enemy
        if (isActive == false)
        {
            if (!Physics.Linecast(headJoint.transform.position, playerTransform.position) || (Vector3.Distance(transform.position, playerTransform.position)) < activationDistance || HP != maxHP)
            { Activate(); }
        }

        if (isActive == true && hasActivated == false) { Activate(); }

        // lets make sure our treads look forward and are level with the ground
        RaycastHit hit;
        // fire a ray downwards
        if (Physics.Raycast(treadRaycastStart.position, Vector3.down, out hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // make our treads look at it
            treadTransform.LookAt(hit.point);
        }
    }

    // when we are enabled
    private void OnEnable()
    {
        // spawn in our enabled fx
        Instantiate(enableParticle, transform.position, Quaternion.identity, null);
    }

    public override void Activate()
    {
        // we are now active
        isActive = true; hasActivated = true;
        // start our movement coroutine
        StartCoroutine(NavMeshPositionTarget());
        // start our coroutine
        StartCoroutine(MainAttack());
    }

    IEnumerator MainAttack()
    {
        yield return new WaitForSeconds(fireRate);
        // then attack
        Attack();
        // then repeat
        StartCoroutine(MainAttack());
    }

    // bullet instantation for animation triggers
    public override void Attack()
    {
        // attack from all our bullet positions
        foreach (Transform shotPos in shotPositions)
        {
            Instantiate(bulletPrefab, shotPos.position, shotPos.rotation, null);
        }
    }

    // when we take damage
    public override void TakeDamage(int dmg)
    {
        // apply damage
        HP -= dmg;
        // flicker 
        GetComponent<Animator>().speed = 1;
        GetComponent<Animator>().Play("HurtFlash");
        // perform hurt animation
    }

    // our death function
    public override void OnDeath()
    {
        // make sure we call our combat zone's death
        if (combatZone)
        { combatZone.OnDeath(); }
        // then blow up
        Instantiate(deathParticle, transform.position, Quaternion.identity, null);
        // then destroy ourselves
        Destroy(gameObject);
    }

    // on trigger enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && isBomb)
            OnDeath();
    }
}

