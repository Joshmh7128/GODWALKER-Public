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

    public override IEnumerator MainCoroutine()
    {
        // lerp to our target position
        active = true;
        yield return new WaitForSecondsRealtime(behaviourTime);
    }

    // choose a new position on the x axis
    void ChoosePos()
    {
        targetPos = transform.position + new Vector3(Random.Range(-moveDistance, moveDistance), 0, Random.Range(-moveDistance, moveDistance));
    }

    public void FixedUpdate()
    {
        // move if active
        if (active)
        {
            // check if we are going to run into anything
            Vector3 dir = targetPos - transform.position; 
            Ray ray = new Ray(transform.position, dir);

            // if we hit anything, stop moving
            if (Physics.SphereCast(ray, bodyRadius*1.5f, bodyRadius * 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                ChoosePos();
            }

            // other than that, move towards our target position
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);

        }
    }

}
