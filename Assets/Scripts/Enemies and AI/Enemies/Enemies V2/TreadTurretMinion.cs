using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreadTurretMinion : EnemyClass
{
    /// the tread turret minion is a small enemy who fires shots at the player
    /// they move to random close points within a thick radius around the player when they are above 50% HP
    /// when they are below 50% HP they will move away from the player, and attempt to run from them
    /// when they shoot they fire three shots at the player, one direct, one slightly left and one slightly right

    // our health point variables are defined in the EnemyClass; HP and MaxHP are defined in the parent abstract class
    [SerializeField] private Transform playerTransform; // our player's transform
    [SerializeField] private float closeRadiusMin, closeRadiusMax, farRadiusMin, farRadiusMax; // our close and far radii
    [SerializeField] private float x, y, z, rx, rz, headHeight; // our movement variables
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] Transform headJoint, shotPos, treadTransform, treadRaycastStart; // our head joint
    [SerializeField] GameObject bulletPrefab;  // start runs at the start of the gameplay
    bool isActive; // are we active? have we seen the player?

    private void Start()
    {
        // make sure we are not active at the start
        isActive = false;
        
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
        // wait 1 to 3 seconds
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        // if our HP is > 50% choose a position in our close radius, if < 50% choose in our far radius
        // top 50%
        if (HP > maxHP / 2f) { x = Random.Range(closeRadiusMin, closeRadiusMax); z = Random.Range(closeRadiusMin, closeRadiusMax); }        
        // bottom 50%
        if (HP < maxHP / 2f)
        { x = Random.Range(farRadiusMin, farRadiusMax); z = Random.Range(farRadiusMin, farRadiusMax); }
        // shuffle those to be positive or negative within the radii
        rx = Random.Range(0, 2); rz = Random.Range(0, 2); if (rx > 0) { x *= -1; } if (rz > 0) { z *= -1; }
        // get the X and Z of that position and use the Y of our head so that we can walk up slopes (may need refining?)
        y = headHeight;
        navMeshAgent.destination = playerTransform.position + new Vector3(x, y, z);
        // repeat this cycle
        StartCoroutine(NavMeshPositionTarget());
    }

    // update runs once per calculated frame
    private void Update()
    {
        // death condition
        if (HP <= 0)
        { Destroy(gameObject); }

        // rotate our headjoint to look at the player
        headJoint.transform.LookAt(playerTransform);
    }

    // the fixed update runs 60 times per second
    private void FixedUpdate()
    {
        // if we can see the player, activate our enemy
        if (isActive == false)
        { if (!Physics.Linecast(transform.position, playerTransform.position)) { Activate(); } }

        // lets make sure our treads look forward and are level with the ground
        RaycastHit hit; 
        // fire a ray downwards
        if (Physics.Raycast(treadRaycastStart.position, Vector3.down, out hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // make our treads look at it
            treadTransform.LookAt(hit.point);
        }
    }

    void Activate()
    {
        // we are now active
        isActive = true; 
        // start our movement coroutine
        StartCoroutine(NavMeshPositionTarget());
        // play our shooting animation
        GetComponent<Animator>().Play("Shooting Animation");
    }

    // randomizing our animation speed
    public void RandomizeAnimationSpeed()
    {
        GetComponent<Animator>().speed = Random.Range(0.75f, 1.25f);
    }

    // bullet instantation for animation triggers
    public void FireBullet()
    {
        Instantiate(bulletPrefab, shotPos); 
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

   
}
