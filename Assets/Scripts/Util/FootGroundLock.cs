using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootGroundLock : MonoBehaviour
{
    // this makes the feet go to the ground
    [SerializeField] float maxDropDistance, raycastHeight;
    [SerializeField] Vector3 startPos, raycastPos;

    Vector3 deltaPos, gammaPos; // the previous and current vector 3 positions of this object

    RaycastHit hit;

    [SerializeField] Transform controlFoot; // the foot we are controlling

    private void Start()
    {
        // set start foot positions
        FootPosCheck(); SometimesUpdate();
        // start our sometimes update
        StartCoroutine(SometimesUpdateHandler());
    }

    // updates 24/sec
    IEnumerator SometimesUpdateHandler()
    {
        // wait one 24th of a second
        yield return new WaitForSecondsRealtime(0.041f);
        // run the sometimes update
        SometimesUpdate();
        // repeat
        StartCoroutine(SometimesUpdateHandler());
        
    }

    void SometimesUpdate()
    {
        // set the gamma position
        gammaPos = transform.position;
        // compare delta & gamma 
        if (gammaPos != deltaPos)
        {
            // act if changed
            FootPosCheck();
        }
        // set delta
        deltaPos = transform.position;
    }


    private void FootPosCheck()
    {
        raycastPos = transform.position + new Vector3(0, raycastHeight, 0);
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
