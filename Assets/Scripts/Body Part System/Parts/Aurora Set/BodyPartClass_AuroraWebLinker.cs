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

    float buffAmount; // how much we're buffing

    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        weaponManager = PlayerWeaponManager.instance;
    }

    // when we pick this up request a shock shot
    public override void OnBodyPartPickup()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
    }

    // whenever we shoot request a shock shot
    public override void OnWeaponFire()
    {
        weaponManager.currentWeapon.requestShockExplodingShot = true;
        CalculateBuff();
    }

    public override void OnWeaponSwap()
    {
        ManageBuff(buffAmount);
    }

    void CalculateBuff()
    {
        // for every enemy tethered gain +50% damage
        float buff = 0; 
        foreach (PlayerShockExplosionScript shock in projectileManager.loopingShockExplosionScripts)
        {
            if (shock.isTethered)
                buff += 0.5f;
        }

        // then setup the new buff
        ManageBuff(buffAmount, buff);
    }

    void ManageBuff(float lastBuff, float newBuff)
    {
        // remove our previous buff
        weaponManager.currentWeapon.damageMods.Remove(lastBuff);
        // then update our damage mod to match
        weaponManager.currentWeapon.damageMods.Add(newBuff);
        // then set to match
        buffAmount = newBuff;
    }

    void ManageBuff(float lastBuff)
    {
        // remove our previous buff
        if (weaponManager.currentWeapon.damageMods.Contains(lastBuff))
            weaponManager.currentWeapon.damageMods.Remove(lastBuff);
        // then set to match
        buffAmount = 0;
    }


    // on ability use, find a random looping shock explosion
    public override void OnUseAbility()
    {
        foreach (PlayerShockExplosionScript shock in projectileManager.loopingShockExplosionScripts)
        {
            if (!shock.isTethered && shock.doesLoop)
            {
                // build a tether with the first shock explosion we find that can
                shock.BuildTether();
                break;
            }
        }
    }
}
