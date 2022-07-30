using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_TestPistol : WeaponClass
{
    public override void UseWeapon(WeaponUseTypes useType)
    {
        if (useType == WeaponUseTypes.OnDown)
        {
            Debug.Log("Use weapon called on " + gameObject.name);
            Fire(); // shoot our gun
            PlayerInverseKinematicsController.instance.ApplyKickRecoil(); // apply our recoil

        }
    }

    // what happens when we shoot this gun?
    void Fire()
    {
        // fire at the camera's aim target
    }
}
