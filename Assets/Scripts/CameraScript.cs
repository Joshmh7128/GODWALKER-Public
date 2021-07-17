using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // controling variables
    [SerializeField] float aimSensitivity; // how sensitive is our camera
    [SerializeField] Transform headTransform; // the transform of our player's head
    [SerializeField] Transform bodyTransform; // the transform of our player's body
    [SerializeField] float yRotate; // Y rotation float
    [SerializeField] float xRotate; // X rotation float
    [SerializeField] float minYAngle; // min our Y can be (usually negative)
    [SerializeField] float maxYAngle; // max our Y can be (usually positive)

    // player variables
    [SerializeField] Transform aimTarget; // where is the camera looking?
    [SerializeField] Transform moveTarget; // where is the camera moving to?

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // run math to rotate the head of the player as we move the mouse
        yRotate += Input.GetAxis("Mouse Y") * -aimSensitivity * Time.deltaTime;
        // clamp the rotation so we don't go around ourselves
        yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
        // calculate our X rotation
        xRotate += Input.GetAxis("Mouse X") * aimSensitivity * Time.deltaTime;
        // aim the camera
        transform.LookAt(aimTarget.position);
        // apply it
        headTransform.eulerAngles = new Vector3(yRotate, xRotate, 0f);
        // bodyTransform.eulerAngles = new Vector3(bodyTransform.rotation.x, xRotate, bodyTransform.rotation.z);
    }
}
