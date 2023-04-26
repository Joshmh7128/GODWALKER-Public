using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponClass : MonoBehaviour 
{
    /// <summary>
    /// This script will be used to build out weapons for rock hopper
    /// Each weapon will have a set of base values, and then more as we build them out
    /// This allows each weapon to be custom scripted and designed to spec
    /// </summary>
    /// 

    // the positions that we need on very weapon

    [Header("Procedural Animation Variables")]
    public Vector3 weaponKickPos; public Vector3 weaponRecoilRot, bodyRecoilRot, reloadRot; // relative to local position
    public Transform rightHandPos, leftHandPos; // where our right and left hands go on this weapon
    // the cosmetic information of our weapon
    public GameObject weaponModel; // our weapon model saved as a prefab
    [Header("Combat Related")]
    public float firerate; public float remainingFirerate, firerateMod; // how quickly can this weapon fire?
    public Transform muzzleOrigin; // the origin of our muzzle
    // our bullet prefab
    public GameObject bulletPrefab; // we spawn this when we fire
    // ammunition vars
    public float currentMagazine, maxMagazine;
    // recharge max is the amount of time in seconds it takes to regenerate one bullet, recharge rate is the speed of the recharge
    public float rechargeRate, recharge, rechargeMax; 
    public float reloadTime; // the amount of time in seconds the reload takes
    public bool reloading; // are we reloading?
    // our weapon's damage and level
    public float level; // whenever we set the level, update our stats 

    public float damage = 1, damageLevelMultiplier; // our damage and the amount it is modified by upgrades
    public List<float> damageMods; 
    float damageMod
    {
        get 
        { 
            float d = 0;
            foreach (float f in damageMods)
            {
                d += f;
            }
            return d + 1; // make sure we cant do damage * 0
        }
    }

    // our weapon's information
    [Header("Weapon Information")]
    public string weaponName;
    public string customInfo; // the custom info about the weapon
    public float rageMultiplier = 1; // what is our hotness?
    public int weaponTier; // our weapon's tier level

    [Header("Feel Related")]
    public float kickFOV = 90f;
    // audio 
    public AudioSource reloadSource, reloadSourceB; // the audio source which plays our reload sound

    [Header("Recoil and Spread Variables")]
    // recoil for weapons
    public float spreadX, spreadY, spreadMax; // spread on each of these axes
    public float spreadXDelta, spreadYDelta; // the increase on each of these axes
    public float spreadReduct, originalSpreadReduct; // how quickly we return to our original state. changed and used in the player camera controller

    // everything to do with upgrades
    public bool requestDoubleShot, requestHomingShot, requestExplodingShot, requestTeleportShot, requestShockExplodingShot;

    // our UI handler
    public WeaponUIHandler weaponUIHandler;

    // our weapon use types
    public enum WeaponUseTypes
    {
        OnDown, OnHold
    }

    // weapon elements
    public enum WeaponElements
    {
        Normal, Energy, Explosive
    }

    [SerializeField] public WeaponElements weaponElement; // our weapon's element

    // the start that is called on every weapon
    public void Start()
    {
        // get our weapon handler
        weaponUIHandler = GetComponent<WeaponUIHandler>();
        // set our original spread reduct
        originalSpreadReduct = spreadReduct;
        // update our stats
        UpdateStats();

    }

    // the start that is called manually on every weapon
    public abstract void WeaponStart();

    // on enable
    public void OnEnable()
    {
        damageMods.Clear(); // clear our damage mods list just in case we have anything leftover
    }

    public abstract void UseWeapon(WeaponUseTypes useType); // public function assigned to using our weapon

    public virtual void Fire()
    {

        // if there is a double shot request, this is a double shot, then set request to false
        if (requestDoubleShot) {requestDoubleShot = false; }
        // if there is a request for a homing shot, this is a homing shot, then set request to false
        bool isHoming = false; // setup for local use
        if (requestHomingShot) {requestHomingShot = false; isHoming = true; }
        // does this bullet explode?
        bool doesExplode = false;// setup for local use
        if (requestExplodingShot) { requestExplodingShot = false; doesExplode = true; }
        // is this a teleporting shot?
        bool isTeleportShot = false;
        if (requestTeleportShot) { requestTeleportShot = false; isTeleportShot = true; }
        // is this a shocking shot?
        bool isShocking = false;
        if (requestShockExplodingShot) { requestShockExplodingShot = false; isShocking = true; }
   

        // apply our recoil
        ApplyKickRecoil();
        // add spread
        AddSpread();
        // kick our UI
        weaponUIHandler.KickUI(); 
        PlayerCameraController.instance.FOVKickRequest(kickFOV);
        // get our direction to our target
        Vector3 shotDirection = PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position;
        // add to our shot direction based on our spread
        Vector3 modifiedShotDirection = new Vector3(shotDirection.x + Random.Range(-spreadX, spreadX), shotDirection.y + Random.Range(-spreadY, spreadY), shotDirection.z).normalized;
        Vector3 finalShotDir = new Vector3(modifiedShotDirection.x, modifiedShotDirection.y, modifiedShotDirection.z);
        // instantiate and shoot our projectile in that direction
        GameObject bullet = Instantiate(bulletPrefab, muzzleOrigin.position, Quaternion.LookRotation(finalShotDir.normalized), null);
        bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, 0);
        PlayerProjectileScript bulletScript = bullet.GetComponent<PlayerProjectileScript>();

        /// apply any mods to our bullet
        // homing?
        if (isHoming) { bulletScript.isHoming = true; }    
        // exploding?
        if (doesExplode) { bulletScript.doesExplode = true; }
        // teleport?
        if (isTeleportShot) { bulletScript.isTeleportShot = true; }
        // shocking?
        if (isShocking) { bulletScript.doesShockExplode = true; }

        // damage modifiers?
        try { bullet.GetComponent<PlayerProjectileScript>().damage = damage * damageMod; } catch { }
        // rage modifiers?
        try { bullet.GetComponent<PlayerProjectileScript>().rageAdd *= rageMultiplier; } catch { }

        remainingFirerate = firerate + firerateMod;

        // if we're not in godwalker mode, use ammo
        if (!PlayerRageManager.instance.godwalking)
        currentMagazine--;
    } // firing our weapon. special shots like double or homing shots are handled above in public vars
    
    // firing custom shots from our weapon
    public virtual void FireCustom(float deathMulti, float damageMulti)
    {
        Debug.Log("firecustom");
        // if there is a double shot request, this is a double shot, then set request to false
        if (requestDoubleShot) {requestDoubleShot = false; }
        // if there is a request for a homing shot, this is a homing shot, then set request to false
        bool isHoming = false; // setup for local use
        if (requestHomingShot) {requestHomingShot = false; isHoming = true; }
        // does this bullet explode?
        bool doesExplode = false;// setup for local use
        if (requestExplodingShot) { requestExplodingShot = false; doesExplode = true; }
        // is this a teleporting shot?
        bool isTeleportShot = false;
        if (requestTeleportShot) { requestTeleportShot = false; isTeleportShot = true; }
        // is this a shocking shot?
        bool isShocking = false;
        if (requestShockExplodingShot) { requestShockExplodingShot = false; isShocking = true; }
        // apply our recoil
        ApplyKickRecoil();
        // add spread
        AddSpread();
        // kick our UI
        weaponUIHandler.KickUI(); 
        PlayerCameraController.instance.FOVKickRequest(kickFOV);
        // get our direction to our target
        Vector3 shotDirection = PlayerCameraController.instance.AimTarget.position - muzzleOrigin.position;
        // add to our shot direction based on our spread
        Vector3 modifiedShotDirection = new Vector3(shotDirection.x + Random.Range(-spreadX, spreadX), shotDirection.y + Random.Range(-spreadY, spreadY), shotDirection.z).normalized;
        Vector3 finalShotDir = new Vector3(modifiedShotDirection.x, modifiedShotDirection.y, modifiedShotDirection.z);
        // instantiate and shoot our projectile in that direction
        GameObject bullet = Instantiate(bulletPrefab, muzzleOrigin.position, Quaternion.LookRotation(finalShotDir.normalized), null);
        bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, 0);
        PlayerProjectileScript bulletScript = bullet.GetComponent<PlayerProjectileScript>();

        /// apply any mods to our bullet
        // homing?
        if (isHoming) { bulletScript.isHoming = true; }    
        // exploding?
        if (doesExplode) { bulletScript.doesExplode = true; }
        // teleport?
        if (isTeleportShot) { bulletScript.isTeleportShot = true; }
        // shocking?
        if (isShocking) { bulletScript.doesShockExplode = true; }

        // damage modifiers?
        try { bullet.GetComponent<PlayerProjectileScript>().damage = damage * damageMod * damageMulti; } catch { }
        try { bullet.GetComponent<PlayerProjectileScript>().deathTime *= deathMulti; } catch { }
        remainingFirerate = firerate + firerateMod;
        currentMagazine--;
    } // firing our weapon. special shots like double or homing shots are handled above in public vars

    public virtual void FireDoubleShot() // call to fire a double shot
    {
        requestDoubleShot = true;
        Fire();
    }
    public virtual void AddSpread() { } // adding spread to our weapon


    public abstract void Reload(bool instant); // function to reload the weapon

    // every weapon will have kick and recoil, unless they are a melee weapon, in which case we will use a different kind of attack method
    public void ApplyKickRecoil()
    {
        PlayerInverseKinematicsController.instance.ApplyKickRecoil();
    }

    // call this when we drop our weapon
    public void OnDrop()
    {
        weaponUIHandler.OnDrop();
    }

    // what to do if we cancel a reload
    public void CancelReload()
    {
        reloading = false;
        weaponUIHandler.CancelReload();
    }

    public bool updated = false;
    public void UpdateStats()
    {
        if (!updated)
        {
            updated = true;

            if (level > 0)
            {
                // set our damage accordingly
                // Debug.Log("setting stats " + level);
                damage = damage * level;
            }

            if (level <= 0)
            {
                // Debug.LogError(gameObject.name + " WeaponClass has a level of 0, check where this is being created! No stats are being set");
            }
        } 
    }
}
