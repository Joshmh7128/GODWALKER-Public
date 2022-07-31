using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_TestPistol : WeaponClass
{
    // attirbutes of this weapon

    // called when the weapon is used
    public override void UseWeapon(WeaponUseTypes useType)
    {
        // since this is a semi automatic weapon, we want to fire ondown
        if (useType == WeaponUseTypes.OnDown)
        {
            // check if we can fire
            if (remainingFirerate <= 0 && currentMagazine > 0)
            {
                Fire(); // shoot our gun
            }
        }
    }

    // what happens when we shoot this gun?
    void Fire()
    {
        ApplyKickRecoil(); // apply our recoil

        // get our direction to our target
        Vector3 shotDirection = PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position;
        // instantiate and shoot our projectile in that direction
        Instantiate(bulletPrefab, muzzleOrigin.position, Quaternion.LookRotation(shotDirection), null);
        remainingFirerate = firerate;
        currentMagazine--;
        // if we're at 0 ammo then reload
        if (currentMagazine <=0)
        {
            Reload();
        }
    }

    // function to reload the gun
    public override void Reload()
    {
        currentMagazine = maxMagazine;
    }

    // runs every physics frame
    private void FixedUpdate()
    {
        if (remainingFirerate > 0)
        {
            remainingFirerate--;
        }
    }

}
