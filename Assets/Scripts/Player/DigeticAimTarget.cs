using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigeticAimTarget : MonoBehaviour
{
    [SerializeField] Transform targetPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos.position, 10);
    }
}
