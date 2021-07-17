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
    [SerializeField] Transform digeticAimTarget; // where is the camera looking?
    [SerializeField] Transform moveTarget; // where is the camera moving to?

    // visuals
    [SerializeField] LineRenderer rightArmLine;
    [SerializeField] Transform rightArm;
    [SerializeField] LineRenderer leftArmLine;
    [SerializeField] Transform leftArm;

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
        bodyTransform.eulerAngles = new Vector3(0f, xRotate, 0f);

        // zooming in with the camera
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 45, 0.05f);
        }        
        
        if (!Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 70, 0.05f);
        }

        // access our line renderers
        rightArmLine.SetPosition(0, rightArm.position);
        rightArmLine.SetPosition(1, digeticAimTarget.position);

        leftArmLine.SetPosition(0, leftArm.position);
        leftArmLine.SetPosition(1, digeticAimTarget.position);


    }

    private void FixedUpdate()
    {
        // where are we aiming?
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            digeticAimTarget.position = hit.point;
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);
        }
        else
        {
            digeticAimTarget.position = transform.forward * 1000f;
            Debug.DrawRay(transform.position, transform.forward * 1000f, Color.red);
        }


    }
}
