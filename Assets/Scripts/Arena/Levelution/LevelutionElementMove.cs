using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementMove : LevelutionElement
{
    [SerializeField] float waitTime; // how long do we wait
    [SerializeField] Vector3 targetWorldPos; // where do we move?
    [SerializeField] float movementSpeed; // how fast do we move there?

    bool canMove = false;

    // this object waits then moves into place
    public override void ActivateElement()
    {
        canMove = true;
    }

    private void Update()
    {
        // if we can move...
        if (canMove)
        {
            // don't move until we have exhausted our waitTime
            if (waitTime > 0)
                waitTime -= Time.deltaTime;

            // then move
            if (waitTime <= 0)
            {
                if (gameObject.GetComponent<Rigidbody>() == null) transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, movementSpeed * Time.deltaTime);

                Vector3 direction = (targetWorldPos - transform.position);

                if (gameObject.GetComponent<Rigidbody>() != null && Vector3.Distance(transform.position, targetWorldPos) > 0.1f)
                    gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + direction * Time.deltaTime * movementSpeed);

                // when we arrive
                if (Vector3.Distance(transform.position, targetWorldPos) >= 0.1f)
                {
                    if (canMove) Arrive();
                }
            }
        }
    }

    void Arrive()
    {
        canMove = false;

        // enable all child objects
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }
}
