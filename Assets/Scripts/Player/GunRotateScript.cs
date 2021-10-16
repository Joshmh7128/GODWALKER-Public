using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotateScript : MonoBehaviour
{
    [SerializeField] Transform aimTarget;

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.gameObject.GetComponent<CameraScript>().canLook)
        // aim the gun
        transform.LookAt(aimTarget.position);
    }
}
