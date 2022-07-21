using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // script handles movement of the player
    [Header("Movement")]
    Vector3 moveH, moveV, move;
    Transform cameraRig;
    [SerializeField] CharacterController characterController; // our character controller
    [SerializeField] float moveSpeed, gravity, jumpVelocity; // set in editor for controlling
    RaycastHit groundedHit; // checking to see if we have touched the ground
    float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    bool landed;
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;

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
        Cursor.lockState = CursorLockMode.Locked;

        // declare our motion
        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");
        moveV = cameraRig.forward * pAxisV;
        moveH = cameraRig.right * pAxisH;

        if (groundCheckCooldown <= 0)
        {
            Physics.SphereCast(transform.position, playerWidth, Vector3.down, out groundedHit, playerHeight, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        } else if (groundCheckCooldown > 0)
        {
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
        else if (groundedHit.transform != null)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerJumpVelocity = Mathf.Sqrt(jumpVelocity * -3.0f * gravity);
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

}
