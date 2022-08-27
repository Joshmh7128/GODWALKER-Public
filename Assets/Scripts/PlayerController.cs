using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // script handles movement of the player
    [Header("Movement")]
    public Vector3 moveH, moveV, move;
    [SerializeField] CharacterController characterController; // our character controller
    [SerializeField] float moveSpeed, gravity, jumpVelocity, normalMoveMultiplier, sprintMoveMultiplier, aimMoveMultiplier, moveSpeedAdjust; // set in editor for controlling
    RaycastHit groundedHit; // checking to see if we have touched the ground
    public float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    public bool grounded;
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;
    bool canMove = true; // can we move?
    public enum MovementStates { normal, sprinting, aiming}
    public MovementStates movementState;

    [Header("Animation Management")]
    public Transform cameraRig, animationRigParent;
    [SerializeField] float maxRealignAngle; // how far can the player turn before we need to realign
    [SerializeField] float realignSpeed; // how quickly we align
    
    // our weapon management
    PlayerWeaponManager weaponManager;

    // body part related
    PlayerBodyPartManager bodyPartManager;

    // setup our instance
    public static PlayerController instance;
    public void Awake()
    {
        instance = this;
    }

    [Header("Visual FX")]
    // visual fx
    [SerializeField] GameObject jumpVFX;
    [SerializeField] GameObject landVFX;
    [SerializeField] GameObject sprintParticleSystem;
    [SerializeField] GameObject capsulePrefab;

    private void Start()
    {
        // get our camera rig
        cameraRig = PlayerCameraController.instance.cameraRig;
        // get our weapon manager
        weaponManager = PlayerWeaponManager.instance; 
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        // get our bodypart manager
        bodyPartManager = PlayerBodyPartManager.instance;

    }

    // Update is called once per frame
    void Update()
    {
        // process our movement inputs
        if (canMove)
        ProcessMovement();
        // setup our animation parent so that the player faces the correct direction
        ProcessAnimationParentControl();
        // how we control our weapon
        ProcessWeaponControl();
        // reloading
        ProcessReloadControl();
    }

    // our movement function
    void ProcessMovement()
    {
        // declare our motion
        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");
        moveV = cameraRig.forward * pAxisV;
        moveH = cameraRig.right * pAxisH;

        if (groundCheckCooldown <= 0)
        {
            Physics.SphereCast(transform.position, playerWidth, Vector3.down, out groundedHit, playerHeight, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        }

        if (groundCheckCooldown > 0)
        {
            playerJumpVelocity += gravityValue * Time.deltaTime;
            groundCheckCooldown -= Time.deltaTime;
        }

       
        // jump calculations
        gravityValue = gravity;

        if (groundedHit.transform == null)
        {
            playerJumpVelocity += gravityValue * Time.deltaTime;
            grounded = false;
        }

        if (groundedHit.transform != null)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space) && (groundCheckCooldown <= 0))
            {
                playerJumpVelocity = Mathf.Sqrt(-jumpVelocity * gravity);
                groundCheckCooldown = groundCheckCooldownMax; // make sure we set the cooldown check
                // instantiate a visual effect
                Instantiate(jumpVFX, transform.position, jumpVFX.transform.rotation, transform);
                // trigger an on jump effect
                bodyPartManager.CallParts("OnJump");

            }
            else if (!grounded)
            {
                // instantiate a visual effect
                Instantiate(landVFX, transform.position, landVFX.transform.rotation, null);

                playerJumpVelocity = 0f;
                grounded = true;
                // trigger an on land effect
                bodyPartManager.CallParts("OnLand");
            }
        }


        // sprint calculation
        if (Input.GetKeyDown(KeyCode.LeftShift) && pAxisV > 0.1f)
        {
            movementState = MovementStates.sprinting;
            PlayerCameraController.instance.FOVMode = PlayerCameraController.FOVModes.sprinting;
            // call on sprint
            bodyPartManager.CallParts("OnSprint");
        }

        // sprint stopping
        if (movementState == MovementStates.sprinting && (pAxisV <= 0.1f || Input.GetMouseButton(1)))
        {
            movementState = MovementStates.normal;
            PlayerCameraController.instance.FOVMode = PlayerCameraController.FOVModes.normal;
            // call off sprint
            bodyPartManager.CallParts("OffSprint");
        }


        // process our state into the movement speed adjuster
        if (movementState == MovementStates.normal)
        { moveSpeedAdjust = normalMoveMultiplier; sprintParticleSystem.SetActive(false); }        
        if (movementState == MovementStates.sprinting)
        { moveSpeedAdjust = sprintMoveMultiplier; sprintParticleSystem.SetActive(true); }       
        if (movementState == MovementStates.aiming)
        { moveSpeedAdjust = aimMoveMultiplier; sprintParticleSystem.SetActive(false); }

        float finalMoveSpeed = moveSpeed * moveSpeedAdjust;
        // calculate vertical movement
        verticalVelocity = playerJumpVelocity;

        move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));

        move = AdjustVelocityToSlope(move);

        // apply final movement
        characterController.Move(move * Time.deltaTime * finalMoveSpeed);
    }

    RaycastHit adjusterHit;
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out adjusterHit, 2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, adjusterHit.normal);
            var adjustedVelocity = slopeRotation * velocity; // this will align the velocity with the surface

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    void ProcessReloadControl()
    {
        // when we press R, reload our current weapon
        if (Input.GetKeyDown (KeyCode.R) && PlayerWeaponManager.instance.currentWeapon.currentMagazine < PlayerWeaponManager.instance.currentWeapon.maxMagazine)
        {
            // check our IK controller to see if we are reloading
            if (PlayerInverseKinematicsController.instance.reloading == false)
            PlayerWeaponManager.instance.currentWeapon.Reload();
        }
    }

    // our animation parent control
    void ProcessAnimationParentControl()
    {
        // if we're not moving, only align when the absolute value of the difference between our animations is more than our realign angle
        if (move == Vector3.zero)
        {

            animationRigParent.eulerAngles = new Vector3(0f, cameraRig.eulerAngles.y, 0f);

        }

        // if we are moving, make sure to turn the character every frame
        if (move != Vector3.zero)
        { animationRigParent.eulerAngles = new Vector3(0, cameraRig.eulerAngles.y, 0); }
    }

    // weapon control
    void ProcessWeaponControl()
    {
        // fire our current weapon
        if (Input.GetMouseButtonDown(0))
        {
            PlayerWeaponManager.instance.currentWeapon.UseWeapon(WeaponClass.WeaponUseTypes.OnDown);
        }

        // fire our current weapon with hold
        if (Input.GetMouseButton(0))
        {
            PlayerWeaponManager.instance.currentWeapon.UseWeapon(WeaponClass.WeaponUseTypes.OnHold);
        }
    }

    // death
    public void OnPlayerDeath()
    {
        // turn off our animation parent
        animationRigParent.gameObject.SetActive(false);
        // make sure we can't move
        canMove = false; 
        // instantiate a bodypart for each of our current bodyparts inside of a capsule collider explode object
        GameObject headCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.headPartClass.gameObject, headCap.transform);

        GameObject bodyCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.torsoPartClass.gameObject, bodyCap.transform);

        GameObject rightArmCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.rightArmPartClass.gameObject, rightArmCap.transform);       
        
        GameObject leftArmCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.leftArmPartClass.gameObject, leftArmCap.transform);

        GameObject rightLegCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.rightLegPartClass.gameObject, rightLegCap.transform);

        GameObject leftLegCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.leftLegPartClass.gameObject, leftLegCap.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(adjusterHit.point, 0.1f);
    }
}
