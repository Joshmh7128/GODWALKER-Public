using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_AuroraQuantunneller : BodyPartClass
{
    /// effects of this upgrade are:
    /// - While ADS you teleport to your shots (3 second cooldown)
    /// - At the start and end of your teleport Shock all the enemies around you
    /// - The more Enemies you Shock the more charge you build up
    /// - Pressing Ability Button now releases all built up charge in a huge blast
    /// 

    // can we teleport?
    bool teleportActive;
    // how often can we teleport?
    float teleportCooldown, teleportCooldownMax;

    // setup activity
    public override void OnADS() => teleportActive = true;
    public override void OffADS() => teleportActive = false;
    
    // instances
    PlayerController playerController;
    PlayerWeaponManager playerWeaponManager;
    PlayerProjectileManager playerProjectileManager;

    private void Awake()
    {
        playerController = PlayerController.instance;
        playerWeaponManager = PlayerWeaponManager.instance;
        playerProjectileManager = PlayerProjectileManager.instance;
    }

    // countdown
    public void FixedUpdate()
    {
        if (teleportCooldown > 0)
            teleportCooldown -= Time.deltaTime;
    }

    // on shoot
    public override void OnWeaponFire()
    {
        // request a teleport shot
        RequestTeleportShot();
    }

    // request a teleport shot
    void RequestTeleportShot()
    {
        playerWeaponManager.currentWeapon.requestTeleportShot = true;
        // find the shot
        StartCoroutine(FindShot());
    }

    // find our teleport shot and link this to it
    IEnumerator FindShot()
    {
        yield return new WaitForFixedUpdate();

    }

    // the actual teleport action
    public override void TryTeleport(Vector3 teleportPos)
    {
        if (teleportActive && teleportCooldown <= 0)
        {
            // set teleport cooldown
            teleportCooldown = teleportCooldownMax;
            // teleport action on player character
        }
    }

}
