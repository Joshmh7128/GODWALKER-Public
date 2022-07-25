using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInverseKinematicsController : MonoBehaviour
{
    // our IK targets
    [SerializeField] Transform targetRightHand, targetLeftHand, targetLook, targetParent, topSpine;
    [SerializeField] Animator animator;

    // our procedural weapon kick and roll control
    Vector3 weaponKickPos, weaponRecoilRot; // !! ensure these are local + relative!
    [SerializeField] Transform recoilParent; // the transform we modify for weapon kick and recoil

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
        weaponManager = PlayerWeaponManager.instance;
    }

    // use to update our values on fire
    public void ApplyKickRecoil()
    {
        weaponKickPos = weaponManager.currentWeapon.weaponKickPos;
        weaponRecoilRot = weaponManager.currentWeapon.weaponRecoilRot;
        // run applications
        ApplyKick();
        ApplyRecoil();

    }

    // apply the kick to our weapon
    void ApplyKick()
    {
        transform.localPosition = weaponKickPos;
    }

    // apply the rotation of our weapon's recoil
    void ApplyRecoil()
    {
        transform.localEulerAngles = weaponRecoilRot;
    }

    // run every frame. lerp our weapon's local position back to 0
    void ProcessKickLerpBack()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 10f * Time.deltaTime);
    }

    // run every frame. lerp our weapon's local rotation back to 0
    void ProcessRecoilLerpBack()
    {
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, 10f * Time.deltaTime);
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
