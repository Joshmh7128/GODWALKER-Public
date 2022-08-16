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
    [SerializeField] float sphereCastWidth; // the width of our spherecast
    RaycastHit uiCheck, check; // hit is for things we are hitting, check is for environmental low level checks, like UI dynamics etc
    [SerializeField] public Transform AimTarget; // the transform of the object we are using to aim at 

    // setup an instance
    public static PlayerCameraController instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // process our camera inputs
        ProcessCameraControl();
    }

    // runs at physics speed
    private void FixedUpdate()
    {
        // calculate this in the fixed update once every frame
        CalculateCheckPoint();
        // update the aim point
        ProcessAimTarget();
        // update our ui raycast
        ProcessUIRaycast();
    }

    // set the position of our aim target
    void ProcessAimTarget()
    {
        // set it to the point of our check point
        if (check.transform != null)
        {
            AimTarget.position = check.point;
        }
        else
        {
            AimTarget.position = transform.forward * 500f;
        }
    }

    // get our check point
    void CalculateCheckPoint()
    {
        Physics.Raycast(transform.position, transform.forward, out check, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Collide);
    }

    // control our camera via the mouse
    void ProcessCameraControl()
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

    // our forward rayast to check for interactables
    void ProcessUIRaycast()
    {
        // fire a ray forward
        Physics.Raycast(transform.position, transform.forward, out uiCheck, 5f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        // then check for UI triggers
        if (uiCheck.transform.tag == "Item")
        {
            WeaponItemUIHandler handler = uiCheck.transform.gameObject.GetComponent<WeaponItemUIHandler>();
            handler.hitPoint = uiCheck.point;
            handler.showPanel = true;
            PlayerWeaponManager.instance.highlightedWeapon = uiCheck.transform.gameObject.GetComponent<WeaponItemUIHandler>().weapon_Item.gameObject;
        }
    }
}
