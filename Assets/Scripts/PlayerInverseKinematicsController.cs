using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInverseKinematicsController : MonoBehaviour
{
    // our IK targets
    public Transform targetRightHand, targetLeftHand, targetLook, targetParent, topSpine;
    [SerializeField] Animator animator;

    // our procedural weapon kick and roll control
    Vector3 weaponKickPos, weaponRecoilRot, bodyRecoilRot; // !! ensure these are local + relative!
    [SerializeField] Transform recoilParent, lookTargetRecoilParent; // the transform we modify for weapon kick and recoil

    // our weapon manager to assist in procedural animation
    PlayerWeaponManager weaponManager;

    // control
    [SerializeField] bool rightArm, leftArm, look;

    // setup our instance
    public static PlayerInverseKinematicsController instance;
    private void Awake()
    {
        instance = this;
    }

    // start to grab instances needed
    private void Start()
    {
        // get our weapon manager instance
    }

    // use to update our values on fire
    public void ApplyKickRecoil()
    {
        /*
        weaponKickPos = weaponManager.currentWeapon.weaponKickPos;
        weaponRecoilRot = weaponManager.currentWeapon.weaponRecoilRot;
        bodyRecoilRot = weaponManager.currentWeapon.bodyRecoilRot;*/
        // run animation applications
        ApplyKick();
        ApplyRecoil();

        Debug.Log("ApplyKickRecoil called on " + this.name);
    }

    // apply the kick to our weapon
    void ApplyKick()
    {
        recoilParent.localPosition = weaponKickPos;
    }

    // apply the rotation of our weapon's recoil
    void ApplyRecoil()
    {
        recoilParent.localEulerAngles = weaponRecoilRot;
        lookTargetRecoilParent.localEulerAngles = bodyRecoilRot;
    }

    // run every frame. lerp our weapon's local position back to 0
    void ProcessKickLerpBack()
    {
        recoilParent.localPosition = Vector3.Lerp(recoilParent.localPosition, Vector3.zero, 10f * Time.deltaTime);
    }

    // run every frame. lerp our weapon's local rotation back to 0
    void ProcessRecoilLerpBack()
    {
        recoilParent.localRotation = Quaternion.Lerp(recoilParent.localRotation, Quaternion.identity, 10f * Time.deltaTime);
        lookTargetRecoilParent.localRotation = Quaternion.Lerp(lookTargetRecoilParent.localRotation, Quaternion.identity, 10f * Time.deltaTime);
    }

    private void Update()
    {
        // make sure we are properly kicking our weapon
        ProcessKickLerpBack();
        ProcessRecoilLerpBack();

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
