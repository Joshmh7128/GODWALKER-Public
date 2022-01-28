using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrabMinion : EnemyClass
{
    /// the tri shot minion is a small enemy who fires 3 shots at the player
    /// they move to random close points within a thick radius around the player when they are above 50% HP
    /// when they are below 50% HP they will move away from the player, and attempt to run from them
    /// when they shoot they fire three shots at the player, one direct, one slightly left and one slightly right

    // our health point variables are defined in the EnemyClass; HP and MaxHP are defined in the parent abstract class
    [SerializeField] private Transform playerTransform; // our player's transform
    [SerializeField] private float closeRadiusMin, closeRadiusMax, farRadiusMin, farRadiusMax; // our close and far radii
    [SerializeField] private float x, y, z, rx, rz, headHeight; // our movement variables
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] Transform headJoint, shotPos; // our head joint
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform frontRightFootGoal, frontLeftFootGoal, backRightFootGoal, backLeftFootGoal; // our ideal foot positions
    [SerializeField] Transform frontRightFootTarget, frontLeftFootTarget, backRightFootTarget, backLeftFootTarget; // our IK animation targets
    [SerializeField] float maxFootDistanceDelta, footSpeed, currentFootSpeed, stepDistance, stepHeight; // how far away can our foot be before we move our feet to it?
    // start runs at the start of the gameplay
    private void Start()
    {
        // get our player transform
        if (playerTransform == null)
        { playerTransform = UpgradeSingleton.Instance.player.transform; }

        // get our nav mesh agent
        if (navMeshAgent == null)
        { navMeshAgent = GetComponent<NavMeshAgent>(); }

        // unparent our feet targets
        frontRightFootTarget.parent = null;
        frontLeftFootTarget.parent = null;
        backLeftFootTarget.parent = null;
        backRightFootTarget.parent = null;

        // start our coroutine
        StartCoroutine(NavMeshPositionTarget());
    }

    // our movement coroutine
    private IEnumerator NavMeshPositionTarget()
    {
        // wait 1 to 3 seconds
        yield return new WaitForSeconds(Random.Range(1f, 8f));
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
        // move our feet
        FootControl(frontRightFootGoal, frontRightFootTarget); FootControl(frontLeftFootGoal, frontLeftFootTarget);  FootControl(backRightFootGoal, backRightFootTarget); FootControl(backLeftFootGoal, backLeftFootTarget);
        // move our goals
        FootGoalControl(frontRightFootGoal); FootGoalControl(frontLeftFootGoal); FootGoalControl(backRightFootGoal); FootGoalControl(backLeftFootGoal); 
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

        // perform hurt animation

    }


    // control our feet for procedural animation
    void FootControl(Transform footGoal, Transform footTarget)
    {

        // make a var for our final target pos
        Vector3 finalTargetPos; Vector3 forwardFootPos, forwardFootDirection; // get a position that is further forward in the direction of where the foot is 
        // if our foot is too far away, move it towards our target foot position at our footspeed
        if (Vector3.Distance(footGoal.position, footTarget.position) > maxFootDistanceDelta)
        {
            currentFootSpeed = footSpeed;
            forwardFootDirection = footGoal.position - footTarget.position;
            forwardFootPos = footGoal.position + (forwardFootDirection.normalized * stepDistance);
            // now lerp our foot
            footTarget.position = new Vector3(forwardFootPos.x, footGoal.position.y, forwardFootPos.z);
        }
    }

    void FootGoalControl(Transform footGoal)
    {
        // fire a ray downwards from about our knee height to make sure we can walk up a slope
        RaycastHit hit;
        // if we hit the ground
        if (Physics.Raycast(footGoal.position + new Vector3(0, 2, 0), Vector3.down, out hit, 3f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // move our footGoal
            footGoal.position = hit.point;
        }
    }
}
