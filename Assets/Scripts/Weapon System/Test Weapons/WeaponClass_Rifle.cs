using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass_Rifle : WeaponClass
{
    // attributes of this weapon
    public override void WeaponStart()
    {
        throw new System.NotImplementedException();
    }
    // called when the weapon is used
    public override void UseWeapon(WeaponUseTypes useType)
    {
        // since this is a semi automatic weapon, we want to fire ondown
        if (useType == WeaponUseTypes.OnHold && !reloading)
        {
            // check if we can fire
            if (remainingFirerate <= 0 && currentMagazine > 0)
            {
                Fire(); // shoot our gun
            }

            // if we're at 0 ammo then reload
            if (remainingFirerate <= 0 && currentMagazine == 0)
            {
                Reload();
            }
        }
    }

    // what happens when we shoot this gun?
    void Fire()
    {
        ApplyKickRecoil(); // apply our recoil
        AddSpread(); // add spread
        weaponUIHandler.KickUI(); // kick our UI
        PlayerCameraController.instance.FOVKickRequest(kickFOV); // kick our camera
        // get our direction to our target
        Vector3 shotDirection = PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position;
        // add to our shot direction based on our spread
        Vector3 modifiedShotDirection = new Vector3(shotDirection.x + Random.Range(-spreadX, spreadX), shotDirection.y + Random.Range(-spreadY, spreadY), shotDirection.z).normalized;
        // instantiate and shoot our projectile in that direction
        GameObject bullet = Instantiate(bulletPrefab, muzzleOrigin.position, Quaternion.LookRotation(modifiedShotDirection.normalized), null);
        bullet.GetComponent<PlayerProjectileScript>().damage = damage;
        remainingFirerate = firerate;
        currentMagazine--;
    }

    // function to reload the gun
    public override void Reload()
    {
        if (!reloading)
        {
            weaponUIHandler.TriggerReload(reloadTime); // start a reload
            StartCoroutine(ReloadTiming());
        }
    }

    // coroutine to reload the gun
    IEnumerator ReloadTiming()
    {
        reloading = true;
        reloadSourceB.PlayOneShot(reloadSourceB.clip);
        PlayerInverseKinematicsController.instance.ApplyReload();
        // make sure we setup our anim controlled to be chill with us reloading
        yield return new WaitForSeconds(reloadTime);
        PlayerInverseKinematicsController.instance.EndReload();
        // play our reloaded sound
        reloadSource.PlayOneShot(reloadSource.clip);
        currentMagazine = maxMagazine;
        reloading = false;
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
