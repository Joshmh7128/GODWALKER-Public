using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPartModifier : MonoBehaviour
{
    // the weaponclass we'll be modifying
    WeaponClass weaponClass;

    [Header("Modification Values: If any are set, they will modify the weapon")]
    [SerializeField] float fireRateMod;
    [SerializeField] float reloadTimeMod; 
    [SerializeField] float spreadMaxMod; 
    [SerializeField] float spreadDeltaXMod; 
    [SerializeField] float spreadDeltaYMod;
    [SerializeField] float magazineMaxMod;
    [SerializeField] GameObject projectile; // the projectile of the weapon we want to fire

    private void OnEnable()
    {
        ActivateModifiers();
    }

    void ActivateModifiers()
    {
        Debug.Log("modifiers triggering on " + gameObject.name);

        // find the weapon class
        if (transform.root.GetComponent<WeaponClass>())
        weaponClass = transform.root.GetComponent<WeaponClass>();

        // apply modifiers
        if (fireRateMod != 0)
        { weaponClass.firerate += fireRateMod; }

        if (reloadTimeMod != 0)
        { weaponClass.reloadTime += reloadTimeMod; }

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
    }
}
