using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] string weaponType;
    [SerializeField] int ammoAmount; // how much we're restoring
    [SerializeField] GameObject noise; // our noise

    bool used; // else if clause
    // on trigger enter with player, check the ammo count of our name. if it is less than its max, pickup the ammo
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            foreach(GameObject weapon in PlayerWeaponManager.instance.weapons)
            {
                if (weapon.GetComponent<WeaponClass>().weaponName == weaponType)
                {
                    // if we aren't full on ammo
                    if (weapon.GetComponent<WeaponClass>().currentMagazine < weapon.GetComponent<WeaponClass>().maxMagazine && !used)
                    {
                        if (weapon.GetComponent<WeaponClass>().currentMagazine + ammoAmount < weapon.GetComponent<WeaponClass>().maxMagazine)
                        {
                            weapon.GetComponent<WeaponClass>().currentMagazine += ammoAmount;
                            used = true;
                        } else
                        {
                            if (!used)
                            weapon.GetComponent<WeaponClass>().currentMagazine = weapon.GetComponent<WeaponClass>().maxMagazine;
                            used = true;
                        }

                        // spawn noise
                        Instantiate(noise, null);
                        // then destroy
                        Destroy(gameObject);

                    }
                }
            }
        }
    }
}
