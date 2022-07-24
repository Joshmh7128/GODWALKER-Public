using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // get our player controller
    PlayerController playerController;
    Animator animator; // our player animator
    Vector3 animationDirection; // which direction is the player moving?
    bool grounded; // are we grounded?
    enum TargetAnimationStates // list our animation states
    {
        baseLayer,
        Legs_Idle,
        Legs_Run_Forward,
        Legs_Run_Backward,
        Legs_Run_Right,
        Legs_Run_Left,      
        Legs_Run_Forward_Right,
        Legs_Run_Forward_Left,
        Legs_Run_Backward_Right,
        Legs_Run_Backward_Left,
        Legs_Jumping,
        Legs_Falling,

        endLayer // exists for organization

    }

    enum MovementStates
    {
        grounded, jumping, falling, dashing
    }

    [SerializeField] MovementStates movementState; // our current movement state

    [SerializeField] TargetAnimationStates targetAnimationState; // what is our target animation state?

    Vector3 startPosition; // get our start position so that we can actively keep the player locked into it

    [SerializeField] float animationTransitionLerpTime; // the lerp speed at which animation values transition

    // start runs after the awake and before the first update
    private void Start()
    {
        // set player controller to instance
        playerController = PlayerController.instance;
        // set our start position
        startPosition = transform.localPosition;
        // set our animator
        animator = GetComponent<Animator>();
        
    }

    public void Update()
    {
        // align our animation target
        MatchAnimationDirection();
        // map our animations
        MapAnimation();
        // apply our animations
        ApplyAnimation();
    }

    // apply our animations to the animation controller
    void ApplyAnimation()
    {
        // always reset our start position before playing
        transform.localPosition = startPosition;

        // lerp to the current layer
        LerpAnimationLayer(targetAnimationState);
    }

    // map our animations to the direction that the player is moving
    void MapAnimation()
    {
        // set grounded 
        grounded = playerController.grounded;

        // set jumping or falling
        if (!grounded)
        {
            // upward movement
            if (animationDirection.y == 1)
            {
                movementState = MovementStates.jumping;
            }

            // fallng movement
            if (animationDirection.y == -1)
            {
                movementState = MovementStates.falling;
            }
        }

        if (grounded) { movementState = MovementStates.grounded; }

        // all of our movements on the ground
        if (movementState == MovementStates.grounded)
        {
            // idle animation map
            if (animationDirection.x == 0 && animationDirection.z == 0)
            {
                targetAnimationState = TargetAnimationStates.Legs_Idle;
            }

            // forward animation map
            if (animationDirection.x == 0 && animationDirection.z == 1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Forward;
            }

            // backward animation map
            if (animationDirection.x == 0 && animationDirection.z == -1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Backward;
            }

            // right animation map
            if (animationDirection.x == 1 && animationDirection.z == 0)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Right;
            }

            // left animation map
            if (animationDirection.x == -1 && animationDirection.z == 0)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Left;
            }

            // forward right
            if (animationDirection.x == 1 && animationDirection.z == 1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Forward_Right;
            }

            // forward left
            if (animationDirection.x == -1 && animationDirection.z == 1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Forward_Left;
            }

            // backward right
            if (animationDirection.x == 1 && animationDirection.z == -1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Backward_Right;
            }

            // forward left
            if (animationDirection.x == -1 && animationDirection.z == -1)
            {
                targetAnimationState = TargetAnimationStates.Legs_Run_Backward_Left;
            }
        }
   
        // jumping
        if (movementState == MovementStates.jumping)
        {
            targetAnimationState = TargetAnimationStates.Legs_Jumping; // make our character jump
        }        
        
        // falling animation (set to jumping for now)
        if (movementState == MovementStates.falling)
        {
            targetAnimationState = TargetAnimationStates.Legs_Jumping; // make our character jump
        }
    }

    // animation layer lerp and blend
    void LerpAnimationLayer(TargetAnimationStates targetLayer)
    {
        // lerp that layer to 1, lerp all else to 0
        for (int i = 0; i < (int)TargetAnimationStates.endLayer; i++)
        {
            // is this layer the target layer?
            if (i == (int)targetLayer)
            {
                // then lerp to 1
                animator.SetLayerWeight(i, Mathf.Lerp(animator.GetLayerWeight(i), 0.99f, animationTransitionLerpTime * Time.deltaTime));
            }
            // if this is not the target layer
            if (i != (int)targetLayer)
            {
                // then lerp to 0
                animator.SetLayerWeight(i, Mathf.Lerp(animator.GetLayerWeight(i), 0, animationTransitionLerpTime * Time.deltaTime));
            }
        }
    }

    // variables to manually calculate movement direction
    float Horizontal, Forward, Vertical;
    float w, s, a, d;

    // function used to match our animation vector2 to the movement vector of the player
    void MatchAnimationDirection()
    {
        // map our keys to numerical values
        if (Input.GetKey(KeyCode.W))
        { w = 1; }
        else if (!Input.GetKey(KeyCode.W))
        { w = 0; }

        if (Input.GetKey(KeyCode.S))
        { s = -1; } 
        else if (!Input.GetKey(KeyCode.S))
        { s = 0; }

        if (Input.GetKey(KeyCode.D))
        { d = 1; }
        else if (!Input.GetKey(KeyCode.D))
        { d = 0; }

        if (Input.GetKey(KeyCode.A))
        { a = -1; }
        else if (!Input.GetKey(KeyCode.A))
        { a = 0; }

        Horizontal = a + d; Forward = w + s;

        // now calculate our Y based on our vertical movement
        if (playerController.verticalVelocity > 0)
        { Vertical = 1; }
        else if (playerController.verticalVelocity < 0)
        { Vertical = -1; }

        // assign the values to the vector input
        animationDirection = new Vector3(Horizontal, Vertical, Forward);
    }
}
