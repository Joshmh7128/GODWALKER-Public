using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPartModifier : MonoBehaviour
{
    // the weaponclass we'll be modifying
    WeaponClass weaponClass;

    [Header("Modification Values: If any are set, they will modify the weapon")]
    [SerializeField] float fireRateMod;
    [SerializeField] float fireRateMax;
    [SerializeField] float reloadTimeMod, reloadTimeMax; 
    [SerializeField] float spreadMaxMod; 
    [SerializeField] float spreadDeltaXMod; 
    [SerializeField] float spreadDeltaYMod;
    [SerializeField] float magazineMaxMod, magazineMaxModMax;
    [SerializeField] GameObject projectile; // the projectile of the weapon we want to fire
    [SerializeField] string nameMod; // an additional name mod
    [SerializeField] float damageMod;

    private void OnEnable()
    {
        ActivateModifiers();
    }

    void ActivateModifiers()
    {
        // find the weapon class
        if (transform.root.GetComponent<WeaponClass>())
        {
            weaponClass = transform.root.GetComponent<WeaponClass>();
        }

        if (weaponClass)
        {
            // apply modifiers
            if (fireRateMod != 0)
            { weaponClass.firerate += fireRateMod; }

            if (fireRateMod != 0 && fireRateMax != 0)
            { weaponClass.firerate += Random.Range(fireRateMod, fireRateMax); }

            if (reloadTimeMod != 0)
            { weaponClass.reloadTime += reloadTimeMod; }
                      
            if (reloadTimeMod != 0 && reloadTimeMax != 0)
            { weaponClass.reloadTime += Random.Range(reloadTimeMod, reloadTimeMax); }

            if (spreadMaxMod != 0)
            { weaponClass.spreadMax += spreadMaxMod; }

            if (spreadDeltaXMod != 0)
            {
                weaponClass.spreadXDelta += spreadDeltaXMod;
            }

            if (spreadDeltaYMod != 0)
            {
                weaponClass.spreadYDelta += spreadDeltaYMod;
            }

            if (projectile)
            {
                weaponClass.bulletPrefab = projectile;
            }

            if (magazineMaxMod != 0)
            {
                weaponClass.maxMagazine += magazineMaxMod;
                weaponClass.currentMagazine = weaponClass.maxMagazine;
            }

            if (magazineMaxMod != 0 && magazineMaxModMax != 0)
            {
                float m = Random.Range(magazineMaxMod, magazineMaxModMax);
                weaponClass.maxMagazine += m;
                weaponClass.currentMagazine = weaponClass.maxMagazine;
            }

            if (nameMod != "")
            { weaponClass.weaponName += " " + nameMod; }

            if (damageMod != 0)
            {
                weaponClass.damage += damageMod;
            }
        }
    }
}
