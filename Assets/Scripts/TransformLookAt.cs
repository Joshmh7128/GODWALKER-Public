using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLookAt : MonoBehaviour
{
    [SerializeField] Transform targetPos;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetPos);
    }
}
