using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroy : MonoBehaviour
{
    [SerializeField] float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("TimeDestroyCoroutine");
    }

    IEnumerator TimeDestroyCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
