using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionTriShotAI : EnemyClass
{
    /// the tri shot minion is a small enemy who fires 3 shots at the player
    /// they move to random close points within a thick radius around the player when they are above 50% HP
    /// when they are below 50% HP they will move away from the player, and attempt to run from them
    /// when they shoot they fire three shots at the player, one direct, one slightly left and one slightly right

    [SerializeField] float maxHP; // our health point variables. HP is defined in the parent abstract class
    [SerializeField] private Transform playerTransform; // our player's transform
    [SerializeField] private float closeRadiusMin, closeRadiusMax, farRadiusMin, farRadiusMax; // our close and far radii
    [SerializeField] private float x, y, z, rx, rz, headHeight; // our movement variables
    [SerializeField] private NavMeshAgent navMeshAgent;

    // start runs at the start of the gameplay
    private void Start()
    {
        // get our player transform
        if (playerTransform == null)
        { playerTransform = UpgradeSingleton.Instance.player.transform; }

        // get our nav mesh agent
        if (navMeshAgent == null)
        { navMeshAgent = GetComponent<NavMeshAgent>(); }

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


    // when we take damage
    public override void TakeDamage(int dmg)
    {
        // apply damage

        // flicker 

        // perform hurt animation

    }
}
