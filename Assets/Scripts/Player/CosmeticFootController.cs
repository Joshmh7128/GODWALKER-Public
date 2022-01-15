using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticFootController : MonoBehaviour
{
    [SerializeField] Transform hipTransform, kneeTransform; // we will get our hips rotation and apply it to ourselves. when our foot is not on the ground, we can use the rotation of our knees to simulate running motion
    Quaternion rot; RaycastHit hit; // everything to do with our firing and recording of the ground normal vector

    // Update is called once per frame
    void FixedUpdate()
    {
        // if we are on the ground, use the ground for our X rotation and our hips for our Y rotation
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            // our up is hit.normal our forward is the direction our hips are facing
           
            rot.eulerAngles = new Vector3(0f,hit.normal.y,hipTransform.forward.z);
        }

        transform.rotation = rot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position+Vector3.down*0.5f);
    }
}
