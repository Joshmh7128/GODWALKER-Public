using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroOut : MonoBehaviour
{
    public bool cancel;

    // Start is called before the first frame update
    void Start()
    {
        if (!cancel)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void ManualZero()
    {
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}
