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
    public bool discoverOnPickup; // should this item be discovered on pickup and modify the save file?

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

                // if we are discovered on pickup, modify the save file
                if (discoverOnPickup)
                SaveDataHandler.instance.DiscoverWeapon(weapon);

            }
        }
    }

    public void DestroyItem()
    {
        Instantiate(destroyVFX, transform.position, Quaternion.identity, null);
        Destroy(ourCanvas.transform.gameObject);
        Debug.Log("Destroying Item");
        Destroy(gameObject);
    }
}
