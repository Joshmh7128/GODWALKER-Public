using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_TestPistol : WeaponClass
{
    // attirbutes of this weapon

    private void Start()
    {
        firerate = 10f; // fire once every 10 frames
    }

    // called when the weapon is used
    public override void UseWeapon(WeaponUseTypes useType)
    {
        // since this is a semi automatic weapon, we want to fire ondown
        if (useType == WeaponUseTypes.OnDown)
        {
            Debug.Log("Use weapon called on " + gameObject.name);
            // check if we can fire
            if (!(remainingFirerate >= 0))
            {
                Fire(); // shoot our gun
            }
        }
    }

    // what happens when we shoot this gun?
    void Fire()
    {
        remainingFirerate = firerate;
        PlayerInverseKinematicsController.instance.ApplyKickRecoil(); // apply our recoil

        // get our direction to our target
        Vector3 shotDirection = 
        PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position;
    }

    // runs every physics frame
    private void FixedUpdate()
    {
        if (remainingFirerate >= 0)
        {
            remainingFirerate--;
        }
    }

}
