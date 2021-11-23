using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildSelector : MonoBehaviour
{
    [SerializeField] bool chunk;
    [SerializeField] bool obstacle;
    [SerializeField] bool devMode;
    bool hasEnabled; // have we enabled?
    int choice;

    // when this object is enabled
    void OnEnable()
    {
        if (!hasEnabled)
            if (transform.childCount > 0)
            {
                choice = Random.Range(0, transform.childCount);
                transform.GetChild(choice).gameObject.SetActive(true);
                hasEnabled = true;
            }
    }

    private void OnDrawGizmos()
    {
        if (chunk == true)
        {
            Gizmos.DrawWireSphere(transform.position, 3);
        }        
        
        if (obstacle == true)
        {
            Gizmos.DrawWireSphere(transform.position, 30);
        }
    }

    public void Regen()
    {
        transform.GetChild(choice).gameObject.SetActive(false);
        choice = Random.Range(0, transform.childCount);
        transform.GetChild(choice).gameObject.SetActive(true);
    }

}
