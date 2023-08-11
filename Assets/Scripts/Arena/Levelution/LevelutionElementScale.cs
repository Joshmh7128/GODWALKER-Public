using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementScale : LevelutionElement
{
    [SerializeField] float waitTime; // how long do we wait
    [SerializeField] Vector3 targetLocalScale; // our target scale
    [SerializeField] float scaleSpeed; // how fast do we scale?
    [Header("Use starting values?")]
    [SerializeField] bool useX;
    [SerializeField] bool useY, useZ; // should we use our starting values in our target value?

    bool canScale = false;

    // this object waits then moves into place
    public override void ActivateElement()
    {
        Debug.Log("levelution scale element triggered");
        // ensure we set our pre-determined positions
        if (useX) targetLocalScale.x = transform.localScale.x;
        if (useY) targetLocalScale.y = transform.localScale.y;
        if (useZ) targetLocalScale.z = transform.localScale.z;

        canScale = true;
    }

    private void Update()
    {
        // if we can scale...
        if (canScale)
        {
            // increase our scale until we are the desired scale
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetLocalScale, scaleSpeed * Time.fixedDeltaTime);

            // check if we are at or above that scale within a radius of reason
            if (Vector3.Distance(transform.localScale, targetLocalScale) < 0.25f)
                Arrive();
        }
    }

    // run when we reach the size we want to be
    void Arrive()
    {
        canScale = false;

        // enable all child objects
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }
}
