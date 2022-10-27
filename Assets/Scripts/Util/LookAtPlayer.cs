using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] bool lookAtCamera, x, y, z; // do we look at the camera?
    Vector3 target;
    Transform helperTransform;

    PlayerController controller;
    private void Start()
    {
        controller = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        if (!lookAtCamera)
            transform.LookAtPlayer();

        if (lookAtCamera)
            transform.LookAtCamera();

        if (x || y || z)
        {
            // setup what we're looking at
            float lx = 0, ly = 0, lz = 0;
            if (x) lx = controller.transform.position.x; else lx = transform.position.x;
            if (y) ly = controller.transform.position.y; else ly = transform.position.y;
            if (z) lz = controller.transform.position.z; else lz = transform.position.z;

            // direction
            Vector3 dir = new Vector3(lx, ly, lz) - transform.position;

            // look at it
            Quaternion q = Quaternion.LookRotation(dir);
            // set
            transform.rotation = q;
        }
    }
}
