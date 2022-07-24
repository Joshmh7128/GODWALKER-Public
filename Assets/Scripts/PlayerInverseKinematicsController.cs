using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerInverseKinematicsController : MonoBehaviour
{
    // our IK targets
    [SerializeField] Transform targetRightHand, targetLeftHand, targetLook;
    [SerializeField] Animator animator;

    private void OnAnimatorIK(int layerIndex)
    {
        // right hands
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetRightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, targetRightHand.rotation);
    }
}
