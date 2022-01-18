using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoScript : MonoBehaviour
{
    [SerializeField] Transform neckTransform, hipTransform, midTransform; // our target transform

    // Update is called once per frame
    void Update()
    {
        // now we are going to calculate the midpoint of our body
        midTransform.position = (neckTransform.position + hipTransform.position) / 2;
    }
}
