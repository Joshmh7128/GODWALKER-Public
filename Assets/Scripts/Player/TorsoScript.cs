using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoScript : MonoBehaviour
{
    [SerializeField] Transform neckTransform, hipTransform, shoulderRight, shoulderLeft; // pre-defined positions
    [SerializeField] Transform midTransform, backRight, backLeft; // our target transforms calculated from other positions

    // Update is called once per frame
    void Update()
    {
        // now we are going to calculate the midpoint of our body
        midTransform.position = (neckTransform.position + hipTransform.position) / 2;
        // calculate our back positions from the midpoint to the shoulders
        backRight.position = (midTransform.position + shoulderRight.position) / 2;
        backLeft.position = (midTransform.position + shoulderLeft.position) / 2;
    }
}
