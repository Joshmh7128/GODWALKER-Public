using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
    // this script manages the IK functionality of humanoid models and can be used for multiple purposes
    // these systems are designed to work with two different overlaping humanoids to create procedural animations

    protected Animator animator; // our humanoid animator

    public bool ikActive = false; // turn this on or off when needed
    [HeaderAttribute("Use to directly affect Avatar/Controller on this object")]
    public Transform rightHandObj = null; // the right hand target
    public Transform leftHandObj = null; // the left hand target
    public Transform rightFootObj = null; // the right foot target 
    public Transform leftFootObj = null; // the left foot target
    public Transform lookObj = null; // what we want our head to look at
    [HeaderAttribute("Use on Kinematic Definer to Determine results")]
    public Transform rightFootObjGoal = null; // the right foot goal 
    public Transform leftFootObjGoal = null; // the left foot goal
    [HeaderAttribute("Procedural Foot Placement Results")]
    public Transform rightFootResultPos = null;
    public Transform leftFootResultPos = null; // the positions which we get from firing rays down from to place our feet
    Vector3 rightFootGoalPos, leftFootGoalPos; // additionals for lerp

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // if our right foot is not null
        if (rightFootObjGoal != null)
        {
            // our raycast's hit information
            RaycastHit hit;
            // fire a ray down from just above our foot, to see if we can place out walk posiiton
            if (Physics.Raycast(rightFootObjGoal.position + new Vector3(0,1,0), Vector3.down, out hit, 1f))
            {
                // set our foot placement to where the raycast hits
                rightFootGoalPos = hit.point;
            }
            else if (!Physics.Raycast(rightFootObjGoal.position + new Vector3(0, 1, 0), Vector3.down, 1f))
            {
                // set our foot placement to where the raycast hits
                rightFootGoalPos = rightFootObjGoal.position;
            }
        }        
        
        // if our left foot is not null
        if (leftFootObjGoal != null)
        {
            // our raycast's hit information
            RaycastHit hit;
            // fire a ray down from just above our foot, to see if we can place out walk posiiton
            if (Physics.Raycast(leftFootObjGoal.position + new Vector3(0,1,0), Vector3.down, out hit, 1f))
            {
                // set our foot placement to where the raycast hits
                leftFootGoalPos = hit.point;
            }
            else if (!Physics.Raycast(leftFootObjGoal.position + new Vector3(0, 1, 0), Vector3.down, 1f))
            {
                // set our foot placement to where the raycast hits
                leftFootGoalPos = leftFootObjGoal.position;
            }
        }


    }

    private void Update()
    {
        // lerp our feet to their goal positions
        if (rightFootObjGoal != null)
        { rightFootResultPos.position = Vector3.Lerp(rightFootResultPos.position, rightFootGoalPos, Time.deltaTime * 20f); }

        if (leftFootObjGoal != null)
        { leftFootResultPos.position = Vector3.Lerp(leftFootResultPos.position, leftFootGoalPos, Time.deltaTime * 20f); }
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }               
                
                // Set the left hand target position and rotation, if one has been assigned
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }        
                
                // Set the right foot target position and rotation, if one has been assigned
                if (rightFootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
                }         
                // Set the left foot target position and rotation, if one has been assigned
                if (leftFootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
                }              
            }
        }
    }
}