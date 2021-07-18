using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollowScript : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    // Update is called once per frame
    void Update()
    {
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
