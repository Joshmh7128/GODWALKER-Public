using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollowScript : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] bool followX = true, followY = true, followZ = true, rotate = true;

    // Update is called once per frame
    void Update()
    {
        // new vector3
        Vector3 goalPos = Vector3.zero;

        if (followX)
        {
            goalPos.x = targetTransform.position.x;
        }

        if (followY)
        {
            goalPos.y = targetTransform.position.y;
        }

        if (followX)
        {
            goalPos.z = targetTransform.position.z;
        }

        if (rotate)
        {
            transform.rotation = targetTransform.rotation;
        }

        // final follow
        transform.position = goalPos;
    }
}
