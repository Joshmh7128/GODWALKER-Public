using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] float aimSensitivity; // how fast the camera aims
    [SerializeField] float minYAngle, maxYAngle; // the minimum and maximum rotations of the camera
    float currentSensitivity, yRotate, xRotate;
    [SerializeField] public Transform cameraRig;
 
    // setup an instance
    public static PlayerCameraController instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // our camera control
        currentSensitivity = aimSensitivity * 10f;
        // run math to rotate the head of the player as we move the mouse
        yRotate += (Input.GetAxis("Mouse Y") * -currentSensitivity * Time.deltaTime);
        // clamp the rotation so we don't go around ourselves
        yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
        // calculate our X rotation
        xRotate += (Input.GetAxis("Mouse X") * currentSensitivity * Time.deltaTime);
        // add in our rotate mods if we have any
        float finalxRotate = xRotate;
        float finalyRotate = yRotate;

        // apply it to our head
        cameraRig.eulerAngles = new Vector3(finalyRotate, finalxRotate, 0f);
    }
}
