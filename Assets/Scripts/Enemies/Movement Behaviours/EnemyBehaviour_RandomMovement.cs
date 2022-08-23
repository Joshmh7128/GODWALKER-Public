using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_RandomMovement : EnemyBehaviour
{
    [SerializeField] float randomMoveRadius;
    [SerializeField] float speed;
    [SerializeField] bool moveAroundPlayer; // should we move around the player?
    Vector3 destPos; // our destination position
    // our main behaviour
    public override IEnumerator MainCoroutine()
    {
        Transform playerTransform = PlayerController.instance.transform;

        // move to a random spot within a range
        destPos = transform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), transform.position.y, Random.Range(-randomMoveRadius, randomMoveRadius));
        // if we move around the player...
        if (moveAroundPlayer)
        {
            destPos = playerTransform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), playerTransform.position.y, Random.Range(-randomMoveRadius, randomMoveRadius));
        }
        enemyClass.navMeshAgent.SetDestination(destPos);
        enemyClass.navMeshAgent.speed = speed;
        // return
        yield return true;
    }
}
