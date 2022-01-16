using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticFootController : MonoBehaviour
{
    [SerializeField] Transform hipTransform, lookTarget; // we will get our hips rotation and apply it to ourselves. when our foot is not on the ground, we can use the rotation of our knees to simulate running motion
    
    Quaternion rot; RaycastHit hit; // everything to do with our firing and recording of the ground normal vector
    Vector3 upNormDif;

    // Update is called once per frame
    void FixedUpdate()
    {
        // if we are on the ground, use the ground for our X rotation and our hips for our Y rotation
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            transform.LookAt(transform.position + hit.normal);
            // lookTarget.position = hipTransform.forward * 2;
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position, newDirection);
    }
}
