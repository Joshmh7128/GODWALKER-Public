using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyRandomLauncher : MonoBehaviour
{
    /// <summary>
    /// Launches a rigidbody in a random direction at a defined force
    /// </summary>
    /// 

    [SerializeField] float force;
    [SerializeField] bool clean;
    Rigidbody rb;
    float count; 

    private void Start()
    {
        // launch the object
        rb = GetComponent<Rigidbody>();
        Vector3 rand = new Vector3(Random.Range(-force, force), Random.Range(-force, force), Random.Range(-force, force));
        // add it
        rb.AddForce(rand, ForceMode.Impulse);
        rb.AddTorque(rand, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        count += Time.deltaTime;

        // if we want to clean
        if (count > 5f && clean)
        {
            // freeze everything
            rb.constraints = RigidbodyConstraints.FreezeAll;
            // disable collider
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
