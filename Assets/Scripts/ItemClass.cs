using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : MonoBehaviour
{
    public bool canGrab;
    // can we be picked up?
    public float pickupDistance = 5, activePickupDistance;

    private void Start()
    {
        activePickupDistance = pickupDistance;
    }

    public virtual void FixedUpdate()
    {
        ProcessCanGrabState();
    }

    public virtual void ProcessCanGrabState()
    {
        // grab check
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < activePickupDistance)
        { 
            canGrab = true;
        }

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > activePickupDistance)
        {
            canGrab = false;
        }

    }
}
