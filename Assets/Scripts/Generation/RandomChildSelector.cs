using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildSelector : MonoBehaviour
{
    [SerializeField] bool chunk;
    [SerializeField] bool devMode;
    int choice;

    // when this object is enabled
    void OnEnable()
    {
        choice = Random.Range(0, transform.childCount);
        transform.GetChild(choice).gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        if (chunk == true)
        {
            Gizmos.DrawWireSphere(transform.position, 3);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (devMode == true)
            {
                Regen();
            }
        }
    }

    public void Regen()
    {
        transform.GetChild(choice).gameObject.SetActive(false);
        choice = Random.Range(0, transform.childCount);
        transform.GetChild(choice).gameObject.SetActive(true);
    }

}
