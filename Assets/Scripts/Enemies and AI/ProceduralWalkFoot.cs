using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalkFoot : MonoBehaviour
{
    [SerializeField] Transform footGoal, footTarget, footLerp; // set all three in inspector
    Vector3 prevTargetPos; // our previous target position
    [SerializeField] float maxFootDistanceDelta; // the maximum distance we can be away from our previous foot position
    [SerializeField] float footSpeed; // how fast can we move our foot?
    [SerializeField] float stepDistance; // how far forward do we step?
    float currentFootSpeed, stepHeight;

    private void Start()
    {
        footTarget.parent = null;
    }

    private void FixedUpdate()
    {
        FootTargetControl();
        FootGoalControl();
        FootLerpControl();
    }

    // control our feet for procedural animation
    void FootTargetControl()
    {
        // make a var for our final target pos
        Vector3 finalTargetPos; Vector3 forwardFootPos, forwardFootDirection; // get a position that is further forward in the direction of where the foot is 
        // if our foot is too far away, move it towards our target foot position at our footspeed
        if (Vector3.Distance(footGoal.position, footTarget.position) > Random.Range(maxFootDistanceDelta, maxFootDistanceDelta*1.5f))
        {
            // set our old target
            prevTargetPos = footGoal.position;
            forwardFootDirection = footGoal.position - footTarget.position;
            forwardFootPos = footGoal.position + (forwardFootDirection.normalized * stepDistance);
            // now lerp our foot
            footTarget.position = new Vector3(forwardFootPos.x, footGoal.position.y, forwardFootPos.z);
        }
    }

    // make sure we are at the correct height
    void FootGoalControl()
    {
        // fire a ray downwards from about our knee height to make sure we can walk up a slope
        RaycastHit hit;
        // if we hit the ground
        if (Physics.Raycast(footGoal.position + new Vector3(0, 3, 0), Vector3.down, out hit, 4f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag != "Enemy")
            // move our footGoal
            footGoal.position = hit.point;
        }
    }

    // control the final lerp of each of our feet from the lerp position to the target
    void FootLerpControl()
    {
        // then lerp to the foot target
        footLerp.position = Vector3.Lerp(footLerp.position, footTarget.position, footSpeed * Time.deltaTime);
    }
}