using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostStartRefresh : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Buffer());
    }
    
    IEnumerator Buffer()
    {
        targetObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        targetObject.SetActive(true);
    }
}
