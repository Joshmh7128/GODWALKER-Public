using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMimic : MonoBehaviour
{
    // point is to mimic another cmera
    [SerializeField] Camera mimic, ourCam;

    private void Update()
    {
        ourCam.fieldOfView = mimic.fieldOfView;
    }
}
