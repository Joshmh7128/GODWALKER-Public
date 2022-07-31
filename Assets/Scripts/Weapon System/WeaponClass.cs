using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponClass : MonoBehaviour 
{
    /// <summary>
    /// This script will be used to build out weapons for rock hopper
    /// Each weapon will have a set of base values, and then more as we build them out
    /// This allows each weapon to be custom scripted and designed to spec
    /// </summary>
    /// 

    // the positions that we need on very weapon
    public Vector3 weaponKickPos, weaponRecoilRot, bodyRecoilRot, reloadRot; // relative to local position
    public Transform rightHandPos, leftHandPos; // where our right and left hands go on this weapon
    // the cosmetic information of our weapon
    public GameObject weaponModel; // our weapon model saved as a prefab
    public float firerate, remainingFirerate; // how quickly can this weapon fire?
    public Transform muzzleOrigin; // the origin of our muzzle
    // our bullet prefab
    public GameObject bulletPrefab; // we spawn this when we fire
    // ammunition vars
    public float currentMagazine, maxMagazine;
    public float reloadTime; // the amount of time in seconds the reload takes
    // audio 
    public AudioSource reloadSource; // the audio source which plays our reload sound

    // our weapon use types
    public enum WeaponUseTypes
    {
        OnDown, OnHold
    }

    public abstract void UseWeapon(WeaponUseTypes useType); // public function assigned to using our weapon

    public abstract void Reload(); // function to reload the weapon

    // every weapon will have kick and recoil, unless they are a melee weapon, in which case we will use a different kind of attack method
    public void ApplyKickRecoil()
    {
        PlayerInverseKinematicsController.instance.ApplyKickRecoil();
    }
}
