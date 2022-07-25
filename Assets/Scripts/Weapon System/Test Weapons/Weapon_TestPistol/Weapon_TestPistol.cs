using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_TestPistol : WeaponClass
{
    public override void UseWeapon()
    {
        Debug.Log("Use weapon called on " + gameObject.name);
        PlayerInverseKinematicsController.instance.ApplyKickRecoil();
    }
}
