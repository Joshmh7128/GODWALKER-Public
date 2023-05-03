using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // script handles movement of the player
    [Header("Movement")]
    public Vector3 moveH, moveV, move, finalMove, processedFinalMove;
    [SerializeField] CharacterController characterController; // our character controller
    public float moveSpeed, gravity, jumpVelocity, normalMoveMultiplier, sprintMoveMultiplier, sprintMoveMod,
        aimMoveMultiplier, moveSpeedAdjust; // set in editor for controlling
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
    public Vector3 knockbackVector; // how much we're getting knocked back
    public float knockbackRecoveryDelta; // how much we recover from knockback

    [Header("Collision and readout")]
    [SerializeField] public float velocity; // our velocity which we only want to read!
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;
    public bool canMove = true; // can we move?
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

    [Header("Movement Ability Management")]
    [SerializeField] PlayerMovementAbilityManager playerMovementAbilityManager;

    [Header("Dash Variables")]
    [SerializeField] float dashLengthMax;
    [SerializeField] float dashRechargeRateDelta, dashCharge, dashUseDelta, dashMultiplier, dashMultiplierMax;
    [SerializeField] GameObject dashVFX, dashSFX; // our dash fx
    bool canDash;

    // our weapon management
    PlayerWeaponManager weaponManager;

    // rage manager
    PlayerRageManager rageManager;

    // setup our instance
    public static PlayerController instance;
    public void Awake()
    {
        // make the player dynamically loadable
        DontDestroyOnLoad(gameObject);
        instance = this;
        // setup bit layer masks
        playerIgnoreMask = LayerMask.NameToLayer("PlayerIgnore");
        ignoreLayerMask = (1 << playerIgnoreMask);
        ignoreLayerMask = ~ignoreLayerMask;
    }

    [Header("Visual FX")]
    // visual fx
    [SerializeField] GameObject jumpVFX, lastJumpVFX;
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
        // rage manager
        rageManager = PlayerRageManager.instance;
        // movement ability manager
        playerMovementAbilityManager = PlayerMovementAbilityManager.instance;

    }

    private void Update()
    {
        ProcessUpdateInputs();

        // resetting the scene
        PrototypeReset();
    }

    // Update is called once per frame
    void FixedUpdate()
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
    }

    void ProcessUpdateInputs()
    {
        // jumping input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (groundedHit.transform != null || remainingJumps > 0 && maxJumps > 0)
            {
                // jumping
                if (Input.GetKey(KeyCode.Space) && (groundCheckCooldown <= 0 || remainingJumps > 0))
                {
                    PlayerRunStatTracker.instance.jumps++;
                    playerJumpVelocity = 0;
                    playerJumpVelocity = Mathf.Sqrt(-jumpVelocity * gravity);
                    remainingJumps--; // reduce jumps
                    groundCheckCooldown = groundCheckCooldownMax; // make sure we set the cooldown check
                    // instantiate a visual effect
                    if (remainingJumps == 0) 
                        Instantiate(lastJumpVFX, transform.position, jumpVFX.transform.rotation, transform);
                    else Instantiate(jumpVFX, transform.position, jumpVFX.transform.rotation, transform);

                }
            }
        }

        // dashing input
        ProcessDash();
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

        // then lerp
        moveV = tmoveV;
        moveH = tmoveH;

        if (groundCheckCooldown <= 0)
        {
            Physics.SphereCast(transform.position, playerWidth, Vector3.down, out groundedHit, playerHeight, ignoreLayerMask, QueryTriggerInteraction.Ignore);
        }

        if (groundCheckCooldown > 0)
        {
            playerJumpVelocity += gravityValue * Time.deltaTime;
            groundCheckCooldown -= Time.deltaTime;
        }

        // jump calculations
        if (gravityMidairMultiplier == 0) { gravityValue = gravity * gravityUpMultiplier * gravityDownMultiplier; } else { gravityValue = gravity * gravityMidairMultiplier; }

        if (groundedHit.transform == null)
        {
            playerJumpVelocity += gravityValue * Time.fixedDeltaTime;
            grounded = false;

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
        float finalMoveSpeed = moveSpeed * moveSpeedAdjust * rageManager.currentSpeedBoost;
        // calculate vertical movement
        verticalVelocity = playerJumpVelocity;

        float moveX = Mathf.Clamp(moveH.x + moveV.x, -1, 1);
        float moveZ = Mathf.Clamp(moveH.z + moveV.z, -1, 1);
        // process all of our final moves
        move = new Vector3(moveX, verticalVelocity / moveSpeed, moveZ);
        move = AdjustVelocityToSlope(move);
        finalMove = Vector3.Lerp(finalMove, move, moveLerpAxisDelta * Time.deltaTime);
        Vector3 clampedFinal = Vector3.ClampMagnitude(new Vector3(finalMove.x, move.y, finalMove.z), 1);
        processedFinalMove = new Vector3(clampedFinal.x, move.y, clampedFinal.z);
        // add our dash movement
        processedFinalMove *= dashMultiplier;
        // knockback processing
        knockbackVector = Vector3.Lerp(knockbackVector, Vector3.zero, knockbackRecoveryDelta * Time.fixedDeltaTime);

        // add knockback vector
        processedFinalMove += knockbackVector;

        // apply final movement
        characterController.Move(processedFinalMove * Time.deltaTime * finalMoveSpeed);

        // output our velocity
        velocity = (Mathf.Abs(finalMove.x) + Mathf.Abs(finalMove.y) + Mathf.Abs(finalMove.z)) * finalMoveSpeed;
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
            PlayerStatManager.instance.StartCoroutine(PlayerStatManager.instance.DeathCountDown());
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
        
        transform.position = teleportPosition + Vector3.up; // move the player up just a little bit so that they don't clip through the ground
        // turn on character controller
        characterController.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(adjusterHit.point, 0.1f);
    }

    public void TriggerKnockback(Vector3 direction, float force)
    {
        // knock us around!
        verticalVelocity = 0;
        knockbackVector = direction.normalized * force;
    }

    // targets for enemies to aim at
    public void ProcessGoWas()
    {
        whereGoTarget.position = transform.position + move * wasGoGamma;
        whereWasTarget.position = transform.position - move * wasGoGamma;
    }

    // process our dash if we have it
    void ProcessDash()
    {

        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");

        // if we have dash left over, use it
        if (Input.GetKey(KeyCode.LeftShift) && playerMovementAbilityManager.movementAbilites[0] && dashCharge > 0 && canDash)
        {
            dashMultiplier = dashMultiplierMax;
            dashCharge -= dashUseDelta * Time.deltaTime; // use our dash charge
            // negate downward or upward movement
            characterController.Move(new Vector3(0, -processedFinalMove.y, 0) * Time.deltaTime);
            // activate our dash fx
            dashVFX.SetActive(true);
            if (!dashSFX.GetComponent<AudioSource>().isPlaying)
            dashSFX.GetComponent<AudioSource>().Play();

            // change fov direction based on movement
            if (pAxisV > 0)
                PlayerCameraController.instance.FOVKickRequest(-0.5f);
            if (pAxisV < 0 || pAxisH != 0)
                PlayerCameraController.instance.FOVKickRequest(0.6f);
        }
        
        // if we press shift and our charge is less than or equal to 0, set our multiplier to 1
        if (Input.GetKey(KeyCode.LeftShift) && playerMovementAbilityManager.movementAbilites[0] && dashCharge <= 0)
        {
            dashMultiplier = 1;
            // deactivate our dash fx
            dashVFX.SetActive(false);
        }

        // when we bring shift up, reset the dash charge
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {            
            if (canDash)
            {
                canDash = false;
                dashCharge = 0;
                dashVFX.SetActive(false);
            }

        }

        // if we are not pressing shift and we can use the dash and our dash charge is less than the dash max
        if (!Input.GetKey(KeyCode.LeftShift) && playerMovementAbilityManager.movementAbilites[0] && dashCharge <= dashLengthMax)
        {
            // when we let go of shift or run our of charge stop dashing and remove our dash
            dashMultiplier = 1;       
        }

        // always recharge the dash
        if (dashCharge <= dashLengthMax)
            dashCharge += dashRechargeRateDelta * Time.deltaTime;
        

        // can we dash?
        if (dashCharge >= dashLengthMax)
        {
            // only reset dash if you let go of the key
            if (!Input.GetKey(KeyCode.LeftShift))
            canDash = true; 
        }

        if (dashCharge <= 0)
        { canDash = false; }


    }
}
