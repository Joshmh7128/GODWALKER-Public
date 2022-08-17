using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroOut : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}
