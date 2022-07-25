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
    public Vector3 weaponKickPos, weaponRecoilRot, bodyRecoilRot; // relative to local position
    public Vector3 rightHandPos, leftHandPos; // where our right and left hands go on this weapon

    public abstract void UseWeapon(); // public function assigned to using our weapon

    // every weapon will have kick and recoil, unless they are a melee weapon, in which case we will use a different kind of attack method
    public void ApplyKickRecoil()
    {
        PlayerInverseKinematicsController.instance.ApplyKickRecoil();
    }
}
