using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalProjectionHandler : MonoBehaviour
{
    [SerializeField] int layermask; // our layermask

    // Start is called before the first frame update
    void Start()
    {
        // put our decal on the ground
        RaycastHit hit;
        // bitshift
        layermask = 1 << layermask;
        // invert
        layermask = ~layermask;
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layermask, QueryTriggerInteraction.Ignore);
        transform.position = hit.point + (Vector3.up * 0.1f);
    }
}
