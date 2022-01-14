using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoScript : MonoBehaviour
{
    [SerializeField] Transform targetTransform; // our target transform

    // Update is called once per frame
    void Update()
    {
        // make sure we are properly moved to our body
        transform.position = targetTransform.position;
    }
}
