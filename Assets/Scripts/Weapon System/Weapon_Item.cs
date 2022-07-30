using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Item : ItemClass
{
    // can we be picked up?
    bool canGrab; 

    // what is our weapon?
    [SerializeField] GameObject weapon;
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
        if (canGrab)
        {
            // replace with Use button later
            if (Input.GetKey(KeyCode.E))
            {
                PlayerWeaponManager.instance.PickupWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canGrab = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canGrab = false;
        }
    }
}
