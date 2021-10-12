using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFOVMatch : MonoBehaviour
{
    private void Update()
    {
        gameObject.GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
    }
}
