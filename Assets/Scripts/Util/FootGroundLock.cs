using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootGroundLock : MonoBehaviour
{
    // this makes the feet go to the ground
    [SerializeField] float maxDropDistance, raycastHeight;
    [SerializeField] Vector3 startPos, raycastPos;
    RaycastHit hit;

    [SerializeField] Transform controlFoot; // the foot we are controlling

    private void FixedUpdate()
    {
        raycastPos = transform.position + new Vector3(0,raycastHeight,0);
        // do our raycast down
        Physics.Raycast(raycastPos, Vector3.down, out hit, 10f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        // move our foot accordingly
        if (hit.transform != null)
        {
            if (Vector3.Distance(transform.position, hit.point) < maxDropDistance)
                controlFoot.position = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, raycastHeight, 0), 0.25f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, -maxDropDistance, 0), 0.25f);
        Gizmos.DrawRay(transform.position + new Vector3(0, raycastHeight, 0), Vector3.down * 10f);
    }

}
