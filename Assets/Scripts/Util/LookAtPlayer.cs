using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] bool lookAtCamera, slowMove; // do we look at the camera? do we move slow?
    [SerializeField] float turnSpeed; // if we are moving slow then use this

    // instance
    PlayerController playerController;

    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        if (!slowMove)
        {
            if (!lookAtCamera)
                transform.LookAtPlayer();

            if (lookAtCamera)
                transform.LookAtCamera();
        }

        // if we are slow moving
        if (slowMove)
        {
            if (!lookAtCamera)
            { 
                // get the look direction
                Vector3 lookDir = transform.position - playerController.transform.position;

                // slerp to the look direction
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed * Time.fixedDeltaTime);
            }

            if (lookAtCamera)
            { 
                // get the look direction
                Vector3 lookDir = transform.position - playerController.cameraRig.transform.position;

                // slerp to the look direction
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
