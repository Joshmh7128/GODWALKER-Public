using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildSelector : MonoBehaviour
{
    [SerializeField] bool chunk;

    // when this object is enabled
    void OnEnable()
    {
        int choice = Random.Range(0, transform.childCount);
        transform.GetChild(choice).gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (chunk == true)
        {
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }

}
