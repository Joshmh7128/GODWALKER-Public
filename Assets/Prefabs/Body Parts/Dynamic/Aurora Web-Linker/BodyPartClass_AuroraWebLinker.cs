using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_AuroraWebLinker : BodyPartClass
{
    ///
    /// - All shots deal Shock damage
    /// - Pressing Ability Button Tethers together all Shocked enemies 
    /// - Tethers deal Shock damage to Enemies
    /// - Damage is now tied to Enemies Tethered
    /// 

    // setup instances
    PlayerWeaponManager weaponManager;
    PlayerProjectileManager projectileManager;
    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        weaponManager = PlayerWeaponManager.instance;
    }

    // whenever we shoot request a shock shot
    public override void OnWeaponFire()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
    }

    // on ability use, find a random looping shock explosion
    public override void OnUseAbility()
    {
        foreach (PlayerShockExplosionScript shock in projectileManager.shockExplosionScripts)
        {
            if (!shock.isTethered)
            {
                // build a tether with the first shock explosion we find that can
                shock.BuildTether();
                break;
            }
        }
    }
}
