using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // get our player controller
    PlayerController playerController;
    Animator animator; // our player animator
    Vector2 animationDirection; // which direction is the player moving?
    enum TargetAnimationStates // list our animation states
    {
        baseLayer,
        Legs_Idle,
        Legs_Run_Forward,
        Legs_Run_Backward,

        endLayer // exists for organization

    }

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
        // idle animation map
        if (animationDirection.x < 0.05f && animationDirection.y < 0.05f)
        {
            targetAnimationState = TargetAnimationStates.Legs_Idle;
        }

        // forward animation map
        if (animationDirection.x < 0.05f && animationDirection.y > 0f)
        {
            targetAnimationState = TargetAnimationStates.Legs_Run_Forward;
        }

        // backward animation map
        if (animationDirection.x < 0.05f && animationDirection.y < -0.1f)
        {
            targetAnimationState = TargetAnimationStates.Legs_Run_Backward;
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

    // function used to match our animation vector2 to the movement vector of the player
    void MatchAnimationDirection()
    {
        animationDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
