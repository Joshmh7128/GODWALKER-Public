using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHandler : MonoBehaviour
{
    // this script manages gates in the multi-arena spaces
    [SerializeField] AudioSource gateAudio; // the source that plays the sound of our gate opening
    [SerializeField] float gateSpeed; // how fast our gate opens
    [SerializeField] Transform gateTransform;
    [SerializeField] Vector3 startGatePos, targetGateLocalPos;
    bool open;

    private void Start()
    {
        // set our start pos
        startGatePos = gateTransform.position;
    }

    // open this gate
    public void OpenGate()
    {
        open = true;
    }

    private void FixedUpdate()
    {
        if (open)
        {
            if (gateAudio.enabled == false)
                gateAudio.enabled = true;

            gateTransform.position = Vector3.MoveTowards(gateTransform.position, startGatePos + targetGateLocalPos, gateSpeed * Time.fixedDeltaTime);
        }
    }
}
