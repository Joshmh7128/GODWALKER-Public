using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalIKHandler : MonoBehaviour
{

    [SerializeField] Transform rightHandTarget, leftHandTarget, rightFootTarget, leftFootTarget;
    [SerializeField] Transform lookTarget;

    [SerializeField] Vector3 lookPos, kickVector;
    [SerializeField] float kickReturnDelta;

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

        if (lookTarget != null)
        {
            animator.SetLookAtPosition(lookPos);
            animator.SetLookAtWeight(1f);
        }

    }

    private void FixedUpdate()
    {
        ProcessLookPos();
    }

    // process our look
    void ProcessLookPos()
    {
        if (lookTarget != null)
        {
            lookPos = lookTarget.position + kickVector;
            // lerp kickvector back to zero
            kickVector = Vector3.Lerp(kickVector, Vector3.zero, kickReturnDelta * Time.deltaTime);
        }
    }

    // kick our look pos
    public void KickLookPos(float kickAmount)
    {
        kickVector = new Vector3(Random.Range(-kickAmount, kickAmount), Random.Range(-kickAmount, kickAmount), Random.Range(-kickAmount, kickAmount));
    }

}
