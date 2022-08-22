using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalIKHandler : MonoBehaviour
{

    [SerializeField] Transform rightHandTarget, leftHandTarget, rightFootTarget, leftFootTarget;
    [SerializeField] Transform lookTarget;

    [SerializeField] Animator animator;

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightHandTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        }

        if (leftHandTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }

        if (rightFootTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        }

        if (leftFootTarget != null)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        }

    }
}
