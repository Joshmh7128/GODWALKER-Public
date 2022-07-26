using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] bool lookAtCamera; // do we look at the camera?

    private void FixedUpdate()
    {
        if (!lookAtCamera)
        transform.LookAt(PlayerController.instance.transform.position);

        if (lookAtCamera)
        transform.LookAt(Camera.main.transform.position);
    }
}
