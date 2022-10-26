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
    public float moveSpeed, gravity, jumpVelocity, normalMoveMultiplier, sprintMoveMultiplier, sprintMoveMod, aimMoveMultiplier, moveSpeedAdjust, groundPoundSpeed; // set in editor for controlling
    [SerializeField] float remainingJumps, maxJumps;
    RaycastHit groundedHit; // checking to see if we have touched the ground
    public float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    public float gravityUpMultiplier = 1, gravityDownMultiplier = 1, gravityMidairMultiplier; // our multipliers for moving up and down with gravity
    public bool grounded;
    public Vector3 lastGroundedPos; // the last position we were at when we were grounded
    float groundTime = 0; // how long we've been grounded



    // dash related
    [Header("Dash Management")]
    public float dashSpeed; // how fast we move when dashing
    public float dashTime, dashTimeMax; // how long we dash for
    public bool dashing = false; // are we currently dashing
    public float dashCooldown, dashCooldownMax;

    [Header("Collision and readout")]
    [SerializeField] public float velocity; // our velocity which we only want to read!
    [SerializeField] float playerHeight, playerWidth; // how tall is the player?
    [SerializeField] float groundCheckCooldown, groundCheckCooldownMax;
    bool canMove = true; // can we move?
    public enum MovementStates { normal, sprinting, aiming}
    public MovementStates movementState;
    int playerIgnoreMask;
    int ignoreLayerMask;

    [Header("Animation Management")]
    public Transform cameraRig, animationRigParent;
    [SerializeField] float maxRealignAngle; // how far can the player turn before we need to realign
    [SerializeField] float realignSpeed; // how quickly we align
    [SerializeField] List<GameObject> dashVFX; // our list of dash fx

    // our weapon management
    PlayerWeaponManager weaponManager;

    // body part related
    PlayerBodyPartManager bodyPartManager;
    // ground pound
    [SerializeField] bool groundPounding;
    [SerializeField] GameObject groundPoundPrefab;
    [SerializeField] GameObject groundPoundJumpFX;
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
        // get our bodypart manager
        bodyPartManager = PlayerBodyPartManager.instance;

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
            // reloading
            ProcessReloadControl();
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

        moveV = animationRigParent.forward * pAxisV;
        moveH = animationRigParent.right * pAxisH;

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
            playerJumpVelocity += gravityValue * Time.deltaTime;
            grounded = false;

            // check and run our midair movements
            bodyPartManager.CallParts("OnMoveMidair"); // we're moving while midair

            // if we were moving up in the last frame 
            if (move.y > 0)
                bodyPartManager.CallParts("OnMoveUp");

            // if we were moving down in the last frame
            if (move.y < 0)
                bodyPartManager.CallParts("OnMoveDown");
        }

        #region // Ground Pound Processing
        if (Input.GetKeyDown(KeyCode.Space) && !grounded && remainingJumps <= 0)
        {
            Instantiate(groundPoundJumpFX, transform.position, groundPoundJumpFX.transform.rotation, null);
            // if we're out of jumps and we press space, we ground pound
            // stop all horizontal movement
            moveH = Vector3.zero; moveV = Vector3.zero;
            // set vertical movement to ground pound speed
            playerJumpVelocity += groundPoundSpeed;
            groundPounding = true;
        }
        #endregion

        if (groundedHit.transform != null || remainingJumps > 0)
        {
            // jumping
            if (Input.GetKeyDown(KeyCode.Space) && (groundCheckCooldown <= 0 || remainingJumps > 0))
            {
                playerJumpVelocity = 0;
                remainingJumps--;
                playerJumpVelocity = Mathf.Sqrt(-jumpVelocity * gravity);
                groundCheckCooldown = groundCheckCooldownMax; // make sure we set the cooldown check
                // instantiate a visual effect
                Instantiate(jumpVFX, transform.position, jumpVFX.transform.rotation, transform);
                // trigger an on jump effect
                bodyPartManager.CallParts("OnJump");
            }
        }        
        
        if (groundedHit.transform != null)
        {
            if (!grounded)
            {
                // instantiate a visual effect
                Instantiate(landVFX, transform.position, landVFX.transform.rotation, null);
                if (groundPounding)
                Instantiate(groundPoundPrefab, transform.position, landVFX.transform.rotation, null);
                remainingJumps = maxJumps;
                playerJumpVelocity = 0f;
                grounded = true;
                groundPounding = false;
                // trigger an on land effect
                bodyPartManager.CallParts("OnLand");
            }
        }

        // sprint calculation
        /*
        if (Input.GetKeyDown(KeyCode.LeftShift) && pAxisV > 0.1f && grounded)
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
        }*/

        #endregion

        #region // Dashing
        // dash calculation
        if (Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(pAxisV) > 0.1f || Mathf.Abs(pAxisH) > 0.1f) && dashTime <= dashTimeMax && dashCooldown <= 0)
        {
            // if we can dash
            if (dashTime <= dashTimeMax)
            {
                // vfx stuff
                PlayerCameraController.instance.FOVKickRequest(95, true);

                UseDash();
                dashing = true; // we are currently dashing
                dashTime += Time.deltaTime; // count up the dash
            }

            // if we're holding and the dash ends
            if (dashTime >= dashTimeMax)
            {
                if (dashCooldown <= 0)
                {
                    dashing = false;
                    playerJumpVelocity = 0;
                    dashCooldown = dashCooldownMax;
                }
            }
        }

        // dashing from stationary
        if (Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(pAxisV) < 0.1f || Mathf.Abs(pAxisH) < 0.1f) && dashTime <= dashTimeMax && dashCooldown <= 0)
        {
            // if we can dash
            if (dashTime <= dashTimeMax)
            {
                // vfx stuff
                PlayerCameraController.instance.FOVKickRequest(95, true);

                UseDash();
                dashing = true; // we are currently dashing
                dashTime += Time.deltaTime; // count up the dash
            }

            // if we're holding and the dash ends
            if (dashTime >= dashTimeMax)
            {
                if (dashCooldown <= 0)
                {
                    dashing = false;
                    playerJumpVelocity = 0;
                    dashCooldown = dashCooldownMax;
                }
            }
        }

        // vfx
        foreach (GameObject g in dashVFX)
        {
            g.SetActive(dashing);
        }

        // dash time calculation
        if (dashTime >= 0 && !dashing)
        {
            dashTime -= Time.deltaTime;
        }


        // dash cancel. if we stop midair, that's it
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            dashing = false;
            dashTime = dashTimeMax;
            // coming out of the end of a dash
            if (dashCooldown <= 0)
            {
                playerJumpVelocity = 0;
                dashCooldown = dashCooldownMax;
            }
        }

        // dash reset
        if (dashing == false)
        {
            dashCooldown -= Time.deltaTime;
        }

        // vfx off
        if (dashTime >= dashTimeMax || !dashing)
        {
            foreach (GameObject g in dashVFX)
            {
                g.SetActive(false);
            }
        }
        #endregion

        #region // Last Grounded Point
        // set our last grounded point
        if (grounded && !dashing)
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

    void UseDash()
    {
        playerJumpVelocity = 0;
        verticalVelocity = 0;
        // declare our motion
        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");
        Vector3 lmoveV = animationRigParent.forward * pAxisV;
        Vector3 lmoveH = animationRigParent.right * pAxisH;
        // lock to horizontal movement
        Vector3 lmove = new Vector3(lmoveH.x + lmoveV.x, -move.y, lmoveH.z + lmoveV.z);
        // move in the direction we're looking
        if (Mathf.Abs(lmove.x) < 0.1f && Mathf.Abs(lmove.z) < 0.1f)
        {
            lmove.x = animationRigParent.forward.normalized.x * moveSpeed * 0.1f;
            lmove.z = animationRigParent.forward.normalized.z * moveSpeed * 0.1f;
        }

        // move character
        characterController.Move(lmove * dashSpeed * Time.deltaTime);
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

    void ProcessReloadControl()
    {
        // when we press R, reload our current weapon
        if (Input.GetKeyDown (KeyCode.R) && PlayerWeaponManager.instance.currentWeapon.currentMagazine < PlayerWeaponManager.instance.currentWeapon.maxMagazine)
        {
            // check our IK controller to see if we are reloading
            if (PlayerInverseKinematicsController.instance.reloading == false)
            PlayerWeaponManager.instance.currentWeapon.Reload(false); // normal, non-instant reload
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
        // explosino instantiation
        Instantiate(deathFX, transform.position, Quaternion.identity, null);

        // instantiate a bodypart for each of our current bodyparts inside of a capsule collider explode object
        GameObject headCap = Instantiate(capsulePrefab, transform.position + Vector3.up, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.headPartClass.gameObject, Vector3.zero, Quaternion.identity, headCap.transform);
        headCap.transform.GetChild(0).transform.localPosition = Vector3.zero;

        GameObject bodyCap = Instantiate(capsulePrefab, transform.position, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.torsoPartClass.gameObject, Vector3.zero, Quaternion.identity, bodyCap.transform);
        bodyCap.transform.GetChild(0).transform.localPosition = Vector3.zero;

        GameObject rightArmCap = Instantiate(capsulePrefab, transform.position + Vector3.right, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.rightArmPartClass.gameObject, Vector3.zero, Quaternion.identity, rightArmCap.transform);
        rightArmCap.transform.GetChild(0).transform.localPosition = Vector3.zero;

        GameObject leftArmCap = Instantiate(capsulePrefab, transform.position + -Vector3.right, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.leftArmPartClass.gameObject, Vector3.zero, Quaternion.identity, leftArmCap.transform);
        leftArmCap.transform.GetChild(0).transform.localPosition = Vector3.zero;

        GameObject rightLegCap = Instantiate(capsulePrefab, transform.position + Vector3.right + Vector3.down, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.rightLegPartClass.gameObject, Vector3.zero, Quaternion.identity, rightLegCap.transform);
        rightLegCap.transform.GetChild(0).transform.localPosition = Vector3.zero;

        GameObject leftLegCap = Instantiate(capsulePrefab, transform.position + -Vector3.right + Vector3.down, Quaternion.identity, null);
        Instantiate(PlayerBodyPartManager.instance.leftLegPartClass.gameObject, Vector3.zero, Quaternion.identity, leftLegCap.transform);
        leftLegCap.transform.GetChild(0).transform.localPosition = Vector3.zero;
    }

    // prototype reset
    public void PrototypeReset()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Fear Prototype");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            SceneManager.LoadScene("Player Controller Testing");
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            Teleport(new Vector3(152, 18, 213));
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
}
