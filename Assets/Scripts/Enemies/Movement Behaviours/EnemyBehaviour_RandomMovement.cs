using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        // if we move around the player...
        if (moveAroundPlayer)
        {
            destPos = playerTransform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), 0, Random.Range(-randomMoveRadius, randomMoveRadius));
            destPos = new Vector3(destPos.x, playerTransform.position.y, destPos.z);
            // be sure to sample the position
            NavMeshHit hit;
            NavMesh.SamplePosition(destPos, out hit, 20, NavMesh.AllAreas);
            destPos = hit.position;
            enemyClass.navMeshAgent.SetDestination(destPos);
            enemyClass.navMeshAgent.speed = speed;
            Debug.Log("setting position around player");
            yield break;

        } 
        
        if (!moveAroundPlayer)
        {
            // move to a random spot within a range
            destPos = transform.position + new Vector3(Random.Range(-randomMoveRadius, randomMoveRadius), 0, Random.Range(-randomMoveRadius, randomMoveRadius));
            // be sure to sample the position before we set it
            NavMeshHit hit;
            NavMesh.SamplePosition(destPos, out hit, 20, NavMesh.AllAreas);
            destPos = hit.position;
            enemyClass.navMeshAgent.SetDestination(destPos);
            enemyClass.navMeshAgent.speed = speed;
            Debug.Log("setting position around self");
            yield break;
        }

        // return
        yield return null;
    }

    private void OnDrawGizmos()
    {
        if (moveAroundPlayer)
            Gizmos.color = Color.green;

        if (!moveAroundPlayer)
            Gizmos.color = Color.red;

        Gizmos.DrawSphere(destPos, 1f);
    }
}
