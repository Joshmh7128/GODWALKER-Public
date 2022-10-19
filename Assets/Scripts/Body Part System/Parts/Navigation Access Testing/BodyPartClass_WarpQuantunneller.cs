using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_WarpQuantunneller : BodyPartClass
{
    // how often can we teleport?
    float teleportCooldown, teleportCooldownMax = 0.25f;

    // on ability use request a teleport
    public override void OnUseAbility()
    {
        RequestTeleportShot();
    }

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
    public override void PartFixedUpdate()
    {
        if (teleportCooldown > 0)
            teleportCooldown -= Time.deltaTime;
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
        // loop through the player projectile manager and find a teleporting shot
        foreach (var projectile in playerProjectileManager.activeProjectileScripts)
        {
            if (projectile.isTeleportShot)
            {
                projectile.teleportCallBack = this;
            }
        }
    }

    // the actual teleport action, called back by the latest fired bullet
    public override void TryTeleport(Vector3 teleportPos)
    {
        Debug.Log("trying teleport");
        // set teleport cooldown
        teleportCooldown = teleportCooldownMax;
        // teleport action on player character
        playerController.Teleport(teleportPos);
    }
}
