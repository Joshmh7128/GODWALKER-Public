using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInverseKinematicsController : MonoBehaviour
{
    // our IK targets
    public Transform targetRightHand, targetLeftHand, targetLook, targetParent, topSpine;
    [SerializeField] Animator animator;
    public bool reloading; // are we reloading?

    // our procedural weapon kick and roll control
    Vector3 weaponKickPos, weaponRecoilRot, bodyRecoilRot, reloadRot; // !! ensure these are local + relative!
    public Transform recoilParent, lookTargetRecoilParent; // the transform we modify for weapon kick and recoil

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

    #region // kick and recoil

    // use to update our values on fire
    public void ApplyKickRecoil()
    {
        weaponKickPos = weaponManager.currentWeapon.weaponKickPos;
        weaponRecoilRot = weaponManager.currentWeapon.weaponRecoilRot;
        bodyRecoilRot = weaponManager.currentWeapon.bodyRecoilRot;
        // run animation applications
        ApplyKick();
        ApplyRecoil();
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

    // when we get hurt, apply similar methods of recoil, called from the PlayerStatManager
    public void ApplyHurtKickRecoil(float kickAmount, float recoilRot, float bodyRecoilRot)
    {
        // move the recoild parent
        recoilParent.localPosition = new Vector3(Random.Range(-kickAmount, kickAmount), Random.Range(-kickAmount, kickAmount), Random.Range(-kickAmount, kickAmount));
        // rotate the angles of the parent
        recoilParent.localEulerAngles = new Vector3(Random.Range(-recoilRot, recoilRot), Random.Range(-recoilRot, recoilRot), Random.Range(-recoilRot, recoilRot));
        // set the angles of the body recoil
        lookTargetRecoilParent.localEulerAngles = new Vector3(Random.Range(-bodyRecoilRot, bodyRecoilRot), Random.Range(-bodyRecoilRot, bodyRecoilRot), Random.Range(-bodyRecoilRot, bodyRecoilRot));
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

    #endregion

    #region // reload animation properties
    // for the application of our reload
    public void ApplyReload()
    {
        reloading = true;
        // set our reload rotation
        reloadRot = weaponManager.currentWeapon.reloadRot;
    }

    public void EndReload()
    {
        reloading = false;
    }

    void ProcessReload()
    {
        // lerp to our current reloadRot
        recoilParent.localPosition = Vector3.Lerp(recoilParent.localPosition, Vector3.zero, 10f * Time.deltaTime);
        recoilParent.localRotation = Quaternion.Lerp(recoilParent.localRotation, Quaternion.Euler(reloadRot), 10f * Time.deltaTime);
    }

    #endregion

    private void Update()
    {
        if (!reloading)
        {
            // make sure we are properly kicking our weapon
            ProcessKickLerpBack();
            ProcessRecoilLerpBack();
        }
        else
        {
            // process our reload when reloading
            ProcessReload();
        }

        targetParent.position = Vector3.Lerp(targetParent.position, topSpine.position, 10f*Time.deltaTime);
        targetParent.LookAt(targetLook);

        // set our hand positions to the gun hand positions
        targetRightHand.position = weaponManager.currentWeapon.rightHandPos.position;
        targetLeftHand.position = weaponManager.currentWeapon.leftHandPos.position;
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
