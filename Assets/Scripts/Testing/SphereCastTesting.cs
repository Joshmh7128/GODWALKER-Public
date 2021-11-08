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

        if (Physics.Linecast(transform.position, targetTransform.position))
        {
            Debug.Log("Linecast hit");
        }

        if (Physics.CheckSphere(targetTransform.position, 0.5f))
        {
            Debug.Log("Checksphere Hit");
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hit.point, 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(Vector3.Lerp(transform.position, hit.point, 0.5f), 0.5f);
        Gizmos.DrawSphere(Vector3.Lerp(transform.position, hit.point, 0.75f), 0.5f);
    }
}
