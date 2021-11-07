using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastTesting : MonoBehaviour
{
    [SerializeField] Transform targetTransform; // our target
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.SphereCast(transform.position, 0.5f, (targetTransform.position - transform.position).normalized, out hit, Vector3.Distance(transform.position, targetTransform.position)))
        {
            Debug.Log("Spherecast Hit");
        }   
    }
}
