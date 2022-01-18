using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMatch : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] bool matchX, matchY, matchZ;
    Quaternion rot;

    private void Update()
    {
        // rotate based on what bools are enabled
        if (matchX)
        { rot.x = targetTransform.rotation.x; }

        if (matchY)
        { rot.y = targetTransform.rotation.y; }

        if (matchZ)
        { rot.z = targetTransform.rotation.z; }

        // set the rotation
        transform.rotation = rot;
    }
}
