using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void FixedUpdate() 
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}
