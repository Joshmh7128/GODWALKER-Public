using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_RandomMovement : EnemyBehaviour
{
    [SerializeField] float randomMoveRadius;
    [SerializeField] float speed;
    // our main behaviour
    public override IEnumerator MainCoroutine()
    {
        // move to a random spot within a range
        Vector3 destPos = transform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), transform.position.y, Random.Range(-randomMoveRadius, randomMoveRadius));
        enemyClass.navMeshAgent.SetDestination(destPos);
        enemyClass.navMeshAgent.speed = speed;
        // return
        yield return true;
    }
}
