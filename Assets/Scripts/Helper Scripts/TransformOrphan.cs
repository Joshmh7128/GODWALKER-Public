using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformOrphan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
    }
}
