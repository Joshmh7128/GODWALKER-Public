using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass_Rifle : WeaponClass
{
    // attirbutes of this weapon

    // called when the weapon is used
    public override void UseWeapon(WeaponUseTypes useType)
    {
        // since this is a semi automatic weapon, we want to fire ondown
        if (useType == WeaponUseTypes.OnHold)
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
        AddSpread(); // add spread
        // get our direction to our target
        Vector3 shotDirection = (PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position).normalized;
        // add to our shot direction based on our spread
        Vector3 modifiedShotDirection = new Vector3(shotDirection.x + Random.Range(-spreadX, spreadX), shotDirection.y + Random.Range(-spreadY, spreadY), shotDirection.z).normalized;
        // instantiate and shoot our projectile in that direction
        Instantiate(bulletPrefab, muzzleOrigin.position, Quaternion.LookRotation(modifiedShotDirection), null);
        remainingFirerate = firerate;
        currentMagazine--;
        // if we're at 0 ammo then reload
        if (currentMagazine <= 0)
        {
            Reload();
        }
    }

    // function to reload the gun
    public override void Reload()
    {
        StartCoroutine(ReloadTiming());
    }

    // coroutine to reload the gun
    IEnumerator ReloadTiming()
    {
        PlayerInverseKinematicsController.instance.ApplyReload();
        // make sure we setup our anim controlled to be chill with us reloading
        yield return new WaitForSeconds(reloadTime);
        PlayerInverseKinematicsController.instance.EndReload();
        // play our reloaded sound
        reloadSource.PlayOneShot(reloadSource.clip);
        currentMagazine = maxMagazine;
    }

    // runs every physics frame
    private void FixedUpdate()
    {
        ProcessFirerate();
        // process our spread
        ProcessSpread();
    }

    void ProcessSpread()
    {
        spreadX = Mathf.Lerp(spreadX, 0, spreadReduct * Time.deltaTime);
        spreadY = Mathf.Lerp(spreadY, 0, spreadReduct * Time.deltaTime);  
    }

    void AddSpread()
    {
        // spread increases overtime and slowly lowers back to normal
        if (spreadX < spreadMax)
        spreadX += spreadXDelta;

        if (spreadY < spreadMax)
        spreadY += spreadYDelta;
    }

    void ProcessFirerate()
    {
        if (remainingFirerate > 0)
        {
            remainingFirerate--;
        }
    }

}
