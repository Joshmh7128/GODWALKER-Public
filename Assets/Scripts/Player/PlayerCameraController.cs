using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] float aimSensitivity;
    [SerializeField] float defaultSensitivity, adsSensitivity, sprintSensitivity; // how fast the camera aims
    [SerializeField] float minYAngle, maxYAngle; // the minimum and maximum rotations of the camera
    float currentSensitivity, yRotate, xRotate;
    [SerializeField] public Transform cameraRig;
    [SerializeField] float sphereCastWidth; // the width of our spherecast
    RaycastHit uiCheck, check; // hit is for things we are hitting, check is for environmental low level checks, like UI dynamics etc
    [SerializeField] public Transform AimTarget; // the transform of the object we are using to aim at 
    [SerializeField] ItemUIHandler handler;
    [SerializeField] Camera mainCam; // our main cam
    [SerializeField] float aimFOV; // how far in we aim
    bool canControl = true; // can we control this?

    [SerializeField] Text aimSensitivityText; // display our current aim sensitivity

    // setup an instance
    public static PlayerCameraController instance;
    PlayerBodyPartManager bodyPartManager;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bodyPartManager = PlayerBodyPartManager.instance;
        // setup our main cam to be referenced
        mainCam = Camera.main;
        // set default sense
        defaultSensitivity = aimSensitivity;

        aimSensitivityText.text = "Current Sensitivity = " + defaultSensitivity;
    }

    private void Update()
    {
        // process our camera inputs
        if (canControl)
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
        // update our camera FOV
        ProcessCameraFOV();
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
            AimTarget.position = mainCam.transform.position + (mainCam.transform.forward * 50f);
        }
    }

    // get our check point
    void CalculateCheckPoint()
    {
        Physics.Raycast(transform.position, transform.forward, out check, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
    }

    // control our camera via the mouse
    void ProcessCameraControl()
    {
        // sensitivity adjuster
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            defaultSensitivity--;
            aimSensitivityText.text = "Current Sensitivity = " + defaultSensitivity;
        }       
        
        if (Input.GetKeyDown(KeyCode.Period))
        {
            defaultSensitivity++;
            aimSensitivityText.text = "Current Sensitivity = " + defaultSensitivity;
        }

        // our camera control
        currentSensitivity = aimSensitivity * 10f;
        // run math to rotate the head of the player as we move the mouse
        yRotate += (Input.GetAxisRaw("Mouse Y") * -currentSensitivity * Time.fixedDeltaTime);
        // clamp the rotation so we don't go around ourselves
        yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);
        // calculate our X rotation
        xRotate += (Input.GetAxisRaw("Mouse X") * currentSensitivity * Time.fixedDeltaTime);
        // add in our rotate mods if we have any
        float finalxRotate = xRotate;
        float finalyRotate = yRotate;

        // apply it to our head
        cameraRig.eulerAngles = new Vector3(finalyRotate, finalxRotate, 0f);

        // aiming if we're not sprinting
        if (FOVMode != FOVModes.sprinting)
        {
            // get our mouse inputs
            if (Input.GetMouseButton(1) && FOVMode == FOVModes.normal)
            {
                PlayerController.instance.movementState = PlayerController.MovementStates.aiming;
                FOVMode = FOVModes.aiming;
                bodyPartManager.CallParts("OnADS");
            }

            if (Input.GetMouseButtonUp(1) && FOVMode == FOVModes.aiming)
            {
                PlayerController.instance.movementState = PlayerController.MovementStates.normal;
                FOVMode = FOVModes.normal;
                bodyPartManager.CallParts("OffADS");
            }
        }
    }

    // our forward rayast to check for interactables
    void ProcessUIRaycast()
    {
        // fire a ray forward
        Physics.Raycast(transform.position, transform.forward, out uiCheck, 5f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        // then check for UI triggers
        if (uiCheck.transform != null)
        {
            // for item UI display
            if (uiCheck.transform.tag == "Item")
            {
                // check if it has a UI handler
                handler = uiCheck.transform.gameObject.GetComponent<ItemUIHandler>();
                handler.hitPoint = uiCheck.point;
                handler.showPanel = true;

                if (handler.itemType == ItemUIHandler.ItemTypes.Weapon)
                    PlayerWeaponManager.instance.highlightedWeapon = uiCheck.transform.gameObject.GetComponent<ItemUIHandler>().weapon_Item.gameObject;

                if (handler.itemType == ItemUIHandler.ItemTypes.BodyPart)
                    PlayerBodyPartManager.instance.highlightedBodyPart = uiCheck.transform.gameObject.GetComponent<ItemUIHandler>().bodyPart_Item.gameObject;
            }
        }


        RaycastHit enemyUICheck;
        Physics.Raycast(transform.position, transform.forward, out enemyUICheck, 90f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        // for enemy UI display
        if (enemyUICheck.transform != null)
        {
            if (enemyUICheck.transform.tag == "Enemy")
            {
                if (enemyUICheck.transform.gameObject.GetComponent<EnemyClass>())
                {
                    enemyUICheck.transform.gameObject.GetComponent<EnemyClass>().showDisplay = true;
                }
            }
        }
    }

    public enum FOVModes
    {
        normal, aiming, sprinting
    }

    public FOVModes FOVMode;

    // for processing any field of view changes
    void ProcessCameraFOV()
    {
        // if we're in normal fov mode, lerp back to normal
        if (FOVMode == FOVModes.normal)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 90f, 3f * Time.deltaTime);
            PlayerWeaponManager.instance.currentWeapon.spreadReduct = PlayerWeaponManager.instance.currentWeapon.originalSpreadReduct;
            aimSensitivity = defaultSensitivity;
        }

        // if we're not in aiming mode
        if (FOVMode == FOVModes.aiming)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, aimFOV, 5f * Time.deltaTime);
            PlayerWeaponManager.instance.currentWeapon.spreadReduct *= 2f;
            aimSensitivity = defaultSensitivity * adsSensitivity;
        }

        // if we're sprinting
        if (FOVMode == FOVModes.sprinting)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 100f, 10f * Time.deltaTime);
            PlayerWeaponManager.instance.currentWeapon.spreadReduct = PlayerWeaponManager.instance.currentWeapon.originalSpreadReduct;
            aimSensitivity = defaultSensitivity * sprintSensitivity;
        }

    }

    public void FOVKickRequest(float fov)
    {
        mainCam.fieldOfView += fov;
    }

    public void FOVKickRequest(float fov, bool hold)
    {
        mainCam.fieldOfView = fov;
    }

    // when the player dies
    public void OnPlayerDeath()
    {
        canControl = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(uiCheck.point, 1f);
    }
}
