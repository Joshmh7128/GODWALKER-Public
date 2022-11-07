using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // randomly enable a child
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(true);
    }

}
