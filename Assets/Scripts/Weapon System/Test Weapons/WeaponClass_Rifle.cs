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
                Reload(false);
            }
        }
    }

    // what happens when we shoot this gun? commented out because it is the same as a pistol
    // public override void Fire() { }

    // function to reload the gun
    public override void Reload(bool instant)
    {
        // our regular reloads
        if (!instant)
        {
            if (!reloading)
            {
                weaponUIHandler.TriggerReload(reloadTime); // start a reload
                StartCoroutine(ReloadTiming(reloadTime));
            }
        }

        // our instance reloads
        if (instant)
        {
            if (!reloading)
            {
                weaponUIHandler.TriggerReload(0.1f); // start a reload
                StartCoroutine(ReloadTiming(0.1f));
            }
        }
    }

    // coroutine to reload the gun
    IEnumerator ReloadTiming(float waitTime)
    {
        reloading = true;
        reloadSourceB.PlayOneShot(reloadSourceB.clip);
        PlayerInverseKinematicsController.instance.ApplyReload();
        // make sure we setup our anim controlled to be chill with us reloading
        yield return new WaitForSeconds(waitTime);
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

    public override void AddSpread()
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
