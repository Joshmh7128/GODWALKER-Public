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
    public float reloadTime; // the amount of time in seconds the reload takes
    public bool reloading; // are we reloading?
    // our weapon's damage and level
    public float level; // whenever we set the level, update our stats 

    public float damage = 1, damageMod, damageLevelMultiplier; // our damage and the amount it is modified by upgrades
    // our weapon's name
    public string weaponName;

    [Header("Feel Related")]
    public float kickFOV = 90f;
    // audio 
    public AudioSource reloadSource, reloadSourceB; // the audio source which plays our reload sound

    [Header("Recoil and Spread Variables")]
    // recoil for weapons
    public float spreadX, spreadY, spreadMax; // spread on each of these axes
    public float spreadXDelta, spreadYDelta; // the increase on each of these axes
    public float spreadReduct, originalSpreadReduct; // how quickly we return to our original state. changed and used in the player camera controller

    // our UI handler
    public WeaponUIHandler weaponUIHandler;

    // our body part manager
    public PlayerBodyPartManager bodyPartManager;

    // our weapon use types
    public enum WeaponUseTypes
    {
        OnDown, OnHold
    }

    // weapon elements
    public enum WeaponElements
    {
        Normal, Fire, Electric, Acid
    }

    [SerializeField] public WeaponElements weaponElement; // our weapon's element

    // the start that is called on every weapon
    public void Start()
    {
        // get our weapon handler
        weaponUIHandler = GetComponent<WeaponUIHandler>();
        // get our body part manager
        bodyPartManager = PlayerBodyPartManager.instance;
        // set our original spread reduct
        originalSpreadReduct = spreadReduct;
        // update our stats
        UpdateStats();

    }

    // the start that is called manually on every weapon
    public abstract void WeaponStart();

    public abstract void UseWeapon(WeaponUseTypes useType); // public function assigned to using our weapon

    public abstract void Fire(); // firing our weapon

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

    public void UpdateStats()
    {
        if (level > 0)
        {
            // set our damage accordingly
            // Debug.Log("setting stats " + level);
            damage *= level;
        }

        if (level <= 0)
        {
            // Debug.LogError(gameObject.name + " WeaponClass has a level of 0, check where this is being created! No stats are being set");
        }
    }
}
