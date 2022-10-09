using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : MonoBehaviour
{
    public bool canGrab;
    // can we be picked up?
    public float pickupDistance = 5;

    public virtual void ProcessCanGrabState()
    {
        // grab check
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < pickupDistance)
        { 
            canGrab = true;
        }

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > pickupDistance)
        {
            canGrab = false;
        }

    }
}
