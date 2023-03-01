using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] bool lookAtCamera, slowMove; // do we look at the camera? do we move slow?
    [SerializeField] float turnSpeed, minDistance, maxDistance; // if we are moving slow then use this
    [SerializeField] bool distanceCompensation; // move at different speeds depending on player distance

    // instance
    PlayerController playerController;

    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        try { PerformLook(); } catch { }
    }


    void PerformLook()
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
                Vector3 lookDir = playerController.transform.position - transform.position;
                float finalSpeed = turnSpeed;

                // calculate our distance compensation
                if (distanceCompensation)
                {
                    // the further the player is the slower we move
                    if (Vector3.Distance(playerController.transform.position, transform.position) > maxDistance)
                    {
                        finalSpeed /= 2;

                    }

                    if (Vector3.Distance(playerController.transform.position, transform.position) < minDistance)
                    {
                        finalSpeed *= 2;
                    }
                }

                // slerp to the look direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir), finalSpeed * Time.fixedDeltaTime);
            }

            if (lookAtCamera)
            {
                // get the look direction
                Vector3 lookDir = playerController.cameraRig.transform.position - transform.position;
                float finalSpeed = turnSpeed;

                // calculate our distance compensation
                if (distanceCompensation)
                {
                    // the further the player is the slower we move
                    if (Vector3.Distance(playerController.transform.position, transform.position) > maxDistance)
                    {
                        finalSpeed /= 3;
                    }

                    if (Vector3.Distance(playerController.transform.position, transform.position) < minDistance)
                    {
                        finalSpeed *= 3;
                    }
                }
                // slerp to the look direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDir), finalSpeed * Time.fixedDeltaTime);
            }
        }
    }
}

