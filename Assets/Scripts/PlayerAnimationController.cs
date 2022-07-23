using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // get our player controller
    PlayerController playerController;
    Animator animator; // our player animator
    Vector2 animationDirection; // which direction is the player moving?
    string targetAnimatiom; // the target animation we want to play

    // start runs after the awake and before the first update
    private void Start()
    {
        // set player controller to instance
        playerController = PlayerController.instance;
        // set our animator
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        // align our animation target
        MatchAnimationDirection();
    }

    void MapAnimation()
    {
        // idle animation map

        // forward animation map
        if (animationDirection == new Vector2(0,1))
        {

        }
    }

    // function used to match our animation vector2 to the movement vector of the player
    void MatchAnimationDirection()
    {
        // translate from 3d to 2d space
        animationDirection = new Vector2(playerController.move.x, playerController.move.z);
    }
}
