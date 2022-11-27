using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // script handles movement of the player
    [Header("Movement")]
    public Vector3 moveH, moveV, move;
    [SerializeField] CharacterController characterController; // our character controller
    public float moveSpeed, gravity, jumpVelocity, normalMoveMultiplier, sprintMoveMultiplier, sprintMoveMod, aimMoveMultiplier, moveSpeedAdjust; // set in editor for controlling
    public float moveLerpAxisDelta;
    RaycastHit groundedHit; // checking to see if we have touched the ground
    public float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    public float gravityUpMultiplier = 1, gravityDownMultiplier = 1, gravityMidairMultiplier; // our multipliers for moving up and down with gravity
    public bool grounded;
    public Vector3 lastGroundedPos; // the last position we were at when we were grounded
    float groundTime = 0; // how long we've been grounded

    [Header("Jump Stuff")]
    public float remainingJumps;
    public float maxJumps;

    [Header("Collision and readout")]
    [SerializeField] public float velocity; // our velocity which we only want to read!
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;
    bool canMove = true; // can we move?
    public enum MovementStates { normal, sprinting, aiming}
    public MovementStates movementState;
    int playerIgnoreMask;
    int ignoreLayerMask;

    [Header("Where Go and Where Was aiming targets")]
    public Transform whereGoTarget; // where we are going 
    public Transform whereWasTarget; // where we were
    [SerializeField] float wasGoGamma; // how much lead to apply

    [Header("Animation Management")]
    public Transform cameraRig, animationRigParent;
    [SerializeField] float maxRealignAngle; // how far can the player turn before we need to realign
    [SerializeField] float realignSpeed; // how quickly we align
    [SerializeField] List<GameObject> dashVFX; // our list of dash fx

    // our weapon management
    PlayerWeaponManager weaponManager;

    // setup our instance
    public static PlayerController instance;
    public void Awake()
    {
        instance = this;
        // setup bit layer masks
        playerIgnoreMask = LayerMask.NameToLayer("PlayerIgnore");
        ignoreLayerMask = (1 << playerIgnoreMask);
        ignoreLayerMask = ~ignoreLayerMask;
    }

    [Header("Visual FX")]
    // visual fx
    [SerializeField] GameObject jumpVFX;
    [SerializeField] GameObject landVFX;
    [SerializeField] GameObject sprintParticleSystem;
    [SerializeField] GameObject capsulePrefab;
    [SerializeField] GameObject deathFX; // our death fx

    private void Start()
    {
        // get our camera rig
        cameraRig = PlayerCameraController.instance.cameraRig;
        // get our weapon manager
        weaponManager = PlayerWeaponManager.instance; 
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        // process our movement inputs
        if (canMove)
        {
            ProcessMovement();
            // setup our animation parent so that the player faces the correct direction
            ProcessAnimationParentControl();
            // how we control our weapon
            ProcessWeaponControl();
            ProcessGoWas();
        }

        // resetting the scene
        PrototypeReset();
    }

    // our movement function
    void ProcessMovement()
    {
        #region // Core Movement
        // declare our motion
        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");
        Vector3 tmoveV = cameraRig.forward * pAxisV;
        Vector3 tmoveH = cameraRig.right * pAxisH;

        moveV = Vector3.Lerp(moveV, tmoveV, moveLerpAxisDelta * Time.fixedDeltaTime);
        moveH = Vector3.Lerp(moveH, tmoveH, moveLerpAxisDelta * Time.fixedDeltaTime);

        if (groundCheckCooldown <= 0)
        {
            Physics.SphereCast(transform.position, playerWidth, Vector3.down, out groundedHit, playerHeight, ignoreLayerMask, QueryTriggerInteraction.Ignore);
        }

        if (groundCheckCooldown > 0)
        {
            playerJumpVelocity += gravityValue * Time.fixedDeltaTime;
            groundCheckCooldown -= Time.deltaTime;
        }

        // jump calculations
        if (gravityMidairMultiplier == 0) { gravityValue = gravity * gravityUpMultiplier * gravityDownMultiplier; } else { gravityValue = gravity * gravityMidairMultiplier; }

        if (groundedHit.transform == null)
        {
            playerJumpVelocity += gravityValue * Time.fixedDeltaTime;
            grounded = false;

        }

        if (groundedHit.transform != null || remainingJumps > 0 && maxJumps > 0)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space) && (groundCheckCooldown <= 0 || remainingJumps > 0))
            {
                playerJumpVelocity = Mathf.Sqrt(-jumpVelocity * gravity);
                remainingJumps--; // reduce jumps
                groundCheckCooldown = groundCheckCooldownMax; // make sure we set the cooldown check
                // instantiate a visual effect
                Instantiate(jumpVFX, transform.position, jumpVFX.transform.rotation, transform);
            }
        }

        if (groundedHit.transform != null)
        {
            if (!grounded)
            {
                // instantiate a visual effect
                Instantiate(landVFX, transform.position, landVFX.transform.rotation, null);
                remainingJumps = maxJumps;
                playerJumpVelocity = 0f;
                grounded = true;
            }
        }

        #endregion

        #region // Last Grounded Point
        // set our last grounded point
        if (grounded)
        {
            groundTime += Time.deltaTime;
            if (groundTime >= 1f)
            lastGroundedPos = transform.position;
        }
        
        // reset groundTime
        if (!grounded || groundTime > 1)
        {
            groundTime = 0;
        }
        #endregion

        #region // Movement State Processing
        // process our state into the movement speed adjuster
        if (movementState == MovementStates.normal)
        { moveSpeedAdjust = normalMoveMultiplier; sprintParticleSystem.SetActive(false); }        
        if (movementState == MovementStates.sprinting)
        { moveSpeedAdjust = sprintMoveMultiplier + sprintMoveMod; sprintParticleSystem.SetActive(true); }       
        if (movementState == MovementStates.aiming)
        { moveSpeedAdjust = aimMoveMultiplier; sprintParticleSystem.SetActive(false); }
        #endregion

        #region // Movement Application
        float finalMoveSpeed = moveSpeed * moveSpeedAdjust;
        // calculate vertical movement
        verticalVelocity = playerJumpVelocity;

        move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));

        move = AdjustVelocityToSlope(move);

        // apply final movement
        characterController.Move(move * Time.deltaTime * finalMoveSpeed);

        // output our velocity
        velocity = (Mathf.Abs(move.x) + Mathf.Abs(move.y) + Mathf.Abs(move.z)) * finalMoveSpeed;
        #endregion
    }
    
    RaycastHit adjusterHit;
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out adjusterHit, 2f, ignoreLayerMask, QueryTriggerInteraction.Ignore))
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
        // explosino instantiation
        Instantiate(deathFX, transform.position, Quaternion.identity, null);

    }

    // prototype reset
    public void PrototypeReset()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("FirstPersonPrototype");
        }

    }

    // teleportation
    public void Teleport(Vector3 teleportPosition)
    {
        // reset velocities
        playerJumpVelocity = 0;
        verticalVelocity = 0;
        // turn off character controller
        characterController.enabled = false;
        StartCoroutine(TeleportBuffer(teleportPosition));
    }

    IEnumerator TeleportBuffer(Vector3 teleportPosition)
    {
        yield return new WaitForFixedUpdate();
        #region // directional method
        /*
        // get a position that is 95% of the way to the new position so that we don't go through any walls
        // get direction, dest - start
        Vector3 dir = teleportPosition - transform.position;
        // get distance
        float distance = Vector3.Distance(transform.position, teleportPosition);
        // distance *= 0.95f; // get 95%
        // get target position relative to zero point
        Vector3 targetPosition = (dir.normalized * distance);
        targetPosition += transform.position; // add our transform position as well to make this a local pos
        // move to new position
        transform.position = targetPosition;*/
        #endregion
        
        transform.position = teleportPosition + Vector3.up; // move the player up just a little bit so that they don't clip through the ground
        // turn on character controller
        characterController.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(adjusterHit.point, 0.1f);
    }

    // targets for enemies to aim at
    public void ProcessGoWas()
    {
        whereGoTarget.position = transform.position + move * wasGoGamma;
        whereWasTarget.position = transform.position - move * wasGoGamma;
    }
}
