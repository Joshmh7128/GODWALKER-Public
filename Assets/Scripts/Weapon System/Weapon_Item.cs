using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Item : ItemClass
{
    // can we be picked up?
    bool canGrab;
    [SerializeField] float pickupDistance = 1;

    // what is our weapon?
    public GameObject weapon;
    [SerializeField] Transform modelParent;

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
        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < pickupDistance)
        {
            if (!PlayerWeaponManager.instance.nearbyWeapons.Contains(gameObject))
            PlayerWeaponManager.instance.nearbyWeapons.Add(gameObject);
            canGrab = true;
        }

        if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) > pickupDistance)
        {
            if (PlayerWeaponManager.instance.nearbyWeapons.Contains(gameObject))
                PlayerWeaponManager.instance.nearbyWeapons.Remove(gameObject);
            canGrab = false;
        }

        // actual grabbing
        if (canGrab)
        {
            // replace with Use button later
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PlayerWeaponManager.instance.pickupCooldown <= 0 && PlayerWeaponManager.instance.nearestWeapon == gameObject)
                {
                    PlayerWeaponManager.instance.nearbyWeapons.Remove(gameObject); // remove this weapon from the list
                    PlayerWeaponManager.instance.PickupWeapon(weapon); // pickup the weapon
                    Destroy(gameObject); // remove the weapon from the world
                }
            }
        }
    }
}
