using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticFootController : MonoBehaviour
{
    [SerializeField] Transform hipTransform, raycastPosForward, cosmeticFoot; // we will get our hips rotation and apply it to ourselves. when our foot is not on the ground, we can use the rotation of our knees to simulate running motion
    
    Quaternion rot; // our desired foot direction
    RaycastHit hit, forwardHit; // everything to do with our firing and recording of the ground normal vector
    Vector3 upNormDif;
    float angle;

    // Update is called once per frame
    void FixedUpdate()
    {
        // if we are on the ground, use the ground for our X rotation and our hips for our Y rotation
        if (Physics.Raycast(transform.position + Vector3.up*0.1f, Vector3.down, out hit, 0.5f))
        {
            // fire a ray downwards from our raycast pos to make our feet look in the proper direction
            Physics.Raycast(raycastPosForward.position, Vector3.down, out forwardHit, 2f);
            Vector3 thighRot = hipTransform.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, thighRot.y, 0);
            cosmeticFoot.LookAt(forwardHit.point);
        }
        else if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.5f))
        {
            Vector3 thighRot = hipTransform.transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, thighRot.y, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + hit.normal);
        Gizmos.color = (Color.red);
    }
}
