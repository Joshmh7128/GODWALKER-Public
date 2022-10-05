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
    private void Awake()
    {
        playerController = PlayerController.instance;
        playerWeaponManager = PlayerWeaponManager.instance;
    }

    // countdown
    public void FixedUpdate()
    {
        if (teleportCooldown > 0)
            teleportCooldown -= Time.deltaTime;
    }

    // request a teleport shot


    // find our teleport shot, store position, and then try to teleport to it


    // the actual teleport action
    void TryTeleport(Vector3 teleportPos)
    {
        if (teleportActive && teleportCooldown <= 0)
        {
            // set teleport cooldown
            teleportCooldown = teleportCooldownMax;
            // teleport action on player character
        }
    }

}
