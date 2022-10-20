using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_RandomAirMovement : EnemyBehaviour
{
    /// midair movement
    /// 

    Vector3 targetPos; // our target position we are always moving to
    bool active; // are we active?
    [SerializeField] float speed; // how fast do we move?
    [SerializeField] float bodyRadius; // how big is our body?
    [SerializeField] float moveDistance; // how far do we move per check?
    [SerializeField] bool drawGizmos;
    [SerializeField] bool playerRelative; // is this relative to the player?

    public override IEnumerator MainCoroutine()
    {
        // lerp to our target position
        active = true;
        yield return new WaitForSecondsRealtime(behaviourTime);
    }

    // choose a new position on the x axis
    void ChoosePos()
    {
        if (!playerRelative)
            targetPos = enemyClass.transform.position + new Vector3(Random.Range(-moveDistance, moveDistance), 0, Random.Range(-moveDistance, moveDistance));

        Vector3 pos = new Vector3(PlayerController.instance.transform.position.x, enemyClass.transform.position.y, PlayerController.instance.transform.position.z);

        if (playerRelative)
            targetPos = pos + new Vector3(Random.Range(-moveDistance, moveDistance), 0, Random.Range(-moveDistance, moveDistance));

    }

    public void FixedUpdate()
    {
        // move if active
        if (active)
        {
            // check if we are going to run into anything
            Vector3 dir = targetPos - enemyClass.transform.position; 
            Ray ray = new Ray(enemyClass.transform.position, dir);

            // if we hit anything, stop moving
            if (Physics.SphereCast(ray, bodyRadius*1.5f, bodyRadius * 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                ChoosePos();
            }

            // other than that, move towards our target position
            enemyClass.transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);

            // when we reach our position, choose a new one
            if (Vector3.Distance(enemyClass.transform.position, targetPos) < bodyRadius)
            {
                ChoosePos();
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPos, 5);
        }
    }

}
