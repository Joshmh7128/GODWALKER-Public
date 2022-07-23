using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // script handles movement of the player
    [Header("Movement")]
    public Vector3 moveH, moveV, move;
    [SerializeField] CharacterController characterController; // our character controller
    [SerializeField] float moveSpeed, gravity, jumpVelocity; // set in editor for controlling
    RaycastHit groundedHit; // checking to see if we have touched the ground
    [SerializeField] float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    bool landed;
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;

    [Header("Animation Management")]
    [SerializeField] Transform cameraRig, animationRigParent;
    [SerializeField] float maxRealignAngle; // how far can the player turn before we need to realign
    

    // setup our instance
    public static PlayerController instance;
    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // get our camera rig
        cameraRig = PlayerCameraController.instance.cameraRig;
    }

    // Update is called once per frame
    void Update()
    {
        // process our movement inputs
        ProcessMovement();
        // setup our animation parent so that the player faces the correct direction
        ProcessAnimationParentControl();
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

        // movement application
        // jump calculations
        gravityValue = gravity;

        if (groundedHit.transform == null)
        {
            playerJumpVelocity += gravityValue * Time.deltaTime;
            landed = false;
        }

        if (groundedHit.transform != null)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space) && (groundCheckCooldown <= 0))
            {
                playerJumpVelocity = Mathf.Sqrt(-jumpVelocity * gravity);
                groundCheckCooldown = groundCheckCooldownMax; // make sure we set the cooldown check
            }
            else if (!landed)
            {
                playerJumpVelocity = 0f;
                landed = true;
            }
        }

        verticalVelocity = playerJumpVelocity;
        move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));
        characterController.Move(move * Time.deltaTime * moveSpeed);
    }

    // our animation parent control
    void ProcessAnimationParentControl()
    {
        // if we're not moving, only align when the absolute value of the difference between our animations is more than our realign angle
        if (move == Vector3.zero)
        {
            if (Mathf.Abs(animationRigParent.eulerAngles.y - cameraRig.eulerAngles.y) > maxRealignAngle)
            {
                animationRigParent.eulerAngles = new Vector3(0, cameraRig.eulerAngles.y, 0);
            }
        }
        
        // if we are moving, make sure to turn the character every frame
        if (move != Vector3.zero)
        { animationRigParent.eulerAngles = new Vector3(0, cameraRig.eulerAngles.y, 0); }
    }
}
