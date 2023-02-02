using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Item : ItemClass
{

    // what is our weapon?
    public GameObject weapon;
    [SerializeField] Transform modelParent;
    [SerializeField] Transform ourCanvas; // so we can make sure it is destroyed
    [SerializeField] GameObject destroyVFX; // the vfx that we make when we get destroyed

    // start
    private void Start()
    {
        // spawn the weapon model in
        GameObject g = Instantiate(weapon.GetComponent<WeaponClass>().weaponModel, modelParent);
        g.transform.localPosition = Vector3.zero;
    }

    // update
    private void Update()
    {
        ProcessCanGrab();
    }

    void ProcessCanGrab()
    {
        // grab check
        if (canGrab)
        {
            if (!PlayerWeaponManager.instance.nearbyWeapons.Contains(gameObject))
                PlayerWeaponManager.instance.nearbyWeapons.Add(gameObject);
        }

        if (!canGrab)
        {
            if (PlayerWeaponManager.instance.nearbyWeapons.Contains(gameObject))
                PlayerWeaponManager.instance.nearbyWeapons.Remove(gameObject);
        }

        // actual grabbing
        if (canGrab)
        {
            // replace with Use button later
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PlayerWeaponManager.instance.pickupCooldown <= 0 && PlayerWeaponManager.instance.highlightedWeapon == gameObject)
                {
                    // PlayerWeaponManager.instance.nearbyWeapons.Remove(gameObject); // remove this weapon from the list
                    PlayerWeaponManager.instance.PickupWeapon(weapon); // pickup the weapon
                    Destroy(ourCanvas.gameObject);
                    Destroy(gameObject); // remove the weapon from the world
                }
            }
        }
    }

    public void DestroyItem()
    {

    }
}
