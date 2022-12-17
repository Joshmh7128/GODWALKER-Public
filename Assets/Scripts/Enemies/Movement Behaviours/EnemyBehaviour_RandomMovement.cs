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
        complete = false;
        Transform playerTransform = PlayerController.instance.transform;

        // move to a random spot within a range
        destPos = transform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), 0, Random.Range(-randomMoveRadius, randomMoveRadius));
        // if we move around the player...
        if (moveAroundPlayer)
        {
            destPos = playerTransform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), 0, Random.Range(-randomMoveRadius, randomMoveRadius));
            destPos = new Vector3(destPos.x, transform.position.y, destPos.z);
        }
        enemyClass.navMeshAgent.SetDestination(destPos);
        enemyClass.navMeshAgent.speed = speed;
        // return
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destPos, 1f);
    }
}
