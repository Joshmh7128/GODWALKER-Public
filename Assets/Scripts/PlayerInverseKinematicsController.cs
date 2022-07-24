using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerInverseKinematicsController : MonoBehaviour
{
    // our IK targets
    [SerializeField] Transform targetRightHand, targetLeftHand, targetLook, targetParent, topSpine;
    [SerializeField] Animator animator;

    // control
    [SerializeField] bool rightArm, leftArm, look;

    private void Update()
    {
        targetParent.position = Vector3.Lerp(targetParent.position, topSpine.position, 10f*Time.deltaTime);
        targetParent.LookAt(targetLook);   
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightArm)
        {   
            // right hands
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetRightHand.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRightHand.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }

        if (leftArm)
        {
            // Left hands
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetLeftHand.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetLeftHand.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }

        if (look)
        {
            // look target
            animator.SetLookAtPosition(targetLook.position);
            animator.SetLookAtWeight(1, 1f, 1, 1, 0);
        }
    }
}
