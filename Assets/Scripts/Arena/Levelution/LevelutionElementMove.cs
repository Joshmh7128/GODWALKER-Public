using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementMove : LevelutionElement
{
    [SerializeField] float waitTime; // how long do we wait
    [SerializeField] Vector3 targetPos; // where do we move?
    [SerializeField] float movementSpeed; // how fast do we move there?

    bool canMove = false;

    // this object waits then moves into place
    public override void ActivateElement()
    {
        canMove = true;
    }

    private void FixedUpdate()
    {
        // if we can move...
        if (canMove)
        {
            // don't move until we have exhausted our waitTime
            if (waitTime > 0)
                waitTime -= Time.fixedDeltaTime;

            // then move
            if (waitTime <= 0)
                transform.position = Vector3.MoveTowards(transform.position, targetPos, movementSpeed * Time.fixedDeltaTime);
        }
    }
}
