using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLerpTo : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 10f);
    }
}
