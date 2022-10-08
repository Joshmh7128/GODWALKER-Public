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
    RaycastHit groundedHit; // checking to see if we have touched the ground
    public float gravityValue, verticalVelocity, playerJumpVelocity; // hidden because is calculated
    public float gravityUpMultiplier = 1, gravityDownMultiplier = 1, gravityMidairMultiplier; // our multipliers for moving up and down with gravity
    public bool grounded;

    // dash related
    [Header("Dash Management")]
    public float dashSpeed; // how fast we move when dashing
    public float dashTime, dashTimeMax; // how long we dash for
    public bool dashing = false; // are we currently dashing

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
            // abilities
            ProcessAbilityControl();
        }

        // resetting the scene
        PrototypeReset();
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && pAxisV > 0.1f && grounded)
        {
            movementState = MovementStates.sprinting;
            PlayerCameraController.instance.FOVMode = PlayerCameraController.FOVModes.sprinting;
            // call on sprint
            bodyPartManager.CallParts("OnSprint");
        }

        // dash calculation
        if (Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(pAxisV) > 0.1f || Mathf.Abs(pAxisH) > 0.1f) && !grounded && dashTime <= dashTimeMax)
        {
            // if we can dash
            if (dashTime <= dashTimeMax)
            {
                // vfx stuff
                PlayerCameraController.instance.FOVKickRequest(95, true);
                if (dashing)
                {
                    foreach (GameObject g in dashVFX)
                    {
                        g.SetActive(true);
                    }
                }

                UseDash();
                dashing = true; // we are currently dashing
                dashTime += Time.deltaTime; // count up the dash
            }
        }

        // dash time calculation
        if (dashTime > 0 && !dashing)
        {
            dashTime -= Time.deltaTime;
        }

        // dash cancel. if we stop midair, that's it
        if (Input.GetKeyUp(KeyCode.LeftShift) && !grounded)
        {
            dashing = false;
            dashTime = dashTimeMax;
        }

        // dash reset
        if (grounded && dashTime > 0)
        {
            dashing = false;
            dashTime = 0;
        }

        // vfx off
        if (dashTime >= dashTimeMax || !dashing)
        {
            foreach (GameObject g in dashVFX)
            {
                g.SetActive(false);
            }
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
        { moveSpeedAdjust = sprintMoveMultiplier + sprintMoveMod; sprintParticleSystem.SetActive(true); }       
        if (movementState == MovementStates.aiming)
        { moveSpeedAdjust = aimMoveMultiplier; sprintParticleSystem.SetActive(false); }

        float finalMoveSpeed = moveSpeed * moveSpeedAdjust;
        // calculate vertical movement
        verticalVelocity = playerJumpVelocity;

        move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));

        move = AdjustVelocityToSlope(move);

        // apply final movement
        characterController.Move(move * Time.deltaTime * finalMoveSpeed);

        // output our velocity
        velocity = (Mathf.Abs(move.x) + Mathf.Abs(move.y) + Mathf.Abs(move.z)) * finalMoveSpeed;

    }

    void UseDash()
    {
        // declare our motion
        float pAxisV = Input.GetAxisRaw("Vertical");
        float pAxisH = Input.GetAxisRaw("Horizontal");
        Vector3 lmoveV = cameraRig.forward * pAxisV;
        Vector3 lmoveH = cameraRig.right * pAxisH;
        // lock to horizontal movement
        Vector3 lmove = new Vector3(lmoveH.x + lmoveV.x, -playerJumpVelocity, lmoveH.z + lmoveV.z);
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

    // ability control
    void ProcessAbilityControl()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (BodyPartClass part in bodyPartManager.bodyParts)
            {
                part.UseAbility(); // use the ability 
            }
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
            SceneManager.LoadScene("Player Controller Testing");
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
