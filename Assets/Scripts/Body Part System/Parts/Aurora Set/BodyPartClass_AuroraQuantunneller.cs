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
    float teleportCooldown, teleportCooldownMax = 0.25f;
    // our charge
    int charge; 


    // setup activity
    public override void OnADS() { teleportActive = true; RequestTeleportShot(); }
    public override void OffADS() => teleportActive = false;

    // shock explosion
    [SerializeField] GameObject shockExplosionPrefab, shockProjectilePrefab;

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
        if (teleportActive && teleportCooldown <= 0)
        {
            playerWeaponManager.currentWeapon.requestTeleportShot = true;
            // find the shot
            StartCoroutine(FindShot());
        }
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
        if (teleportActive && teleportCooldown <= 0)
        {
            // set teleport cooldown
            teleportCooldown = teleportCooldownMax;
            // teleport action on player character
            playerController.Teleport(teleportPos);
            // spawn our explosion at the start and end points
            Instantiate(shockExplosionPrefab, transform.position, Quaternion.identity);
            Instantiate(shockExplosionPrefab, teleportPos, Quaternion.identity);
        }
    }

    // whenever we deal shock damage we build up one charge
    public override void OnShockDamage()
    {
        charge++;
    }

    // when we use our ability discharge everything we've stored up!
    public override void OnUseAbility()
    {
        StartCoroutine(AbilityBuffer());
    }

    // coroutine to discharge
    IEnumerator AbilityBuffer()
    {
        int localCharge = charge;
        charge = 0;
        for (int i = 0; i < localCharge; i++)
        {
            yield return new WaitForSecondsRealtime(1 / 30);
            Instantiate(shockProjectilePrefab, playerController.transform.position + playerController.animationRigParent.forward, playerController.animationRigParent.rotation, null);
        }

        yield return null;
    }

}
