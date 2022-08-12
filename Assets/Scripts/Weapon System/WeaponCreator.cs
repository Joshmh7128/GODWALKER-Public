using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreator : MonoBehaviour
{
    // which weapon maker do we want to create a weapon from?
    public enum WeaponMakers
    {
        Random,
        Valkyrie, 
        Hera,
        Athena,
        Deimos,
        Phobos,

        end
    }

    // the one we have chosen
    [SerializeField] WeaponMakers WeaponMaker;

    // the list of weapons per maker
    [SerializeField] List<GameObject> ValkyrieWeapons = new List<GameObject>();
    [SerializeField] List<GameObject> HeraWeapons = new List<GameObject>();
    [SerializeField] List<GameObject> AthenaWeapons = new List<GameObject>();
    [SerializeField] List<GameObject> DeimosWeapons = new List<GameObject>();
    [SerializeField] List<GameObject> PhobosWeapons = new List<GameObject>();

    // our weapon objects
    GameObject weaponObject;
    [SerializeField] GameObject weaponItem; // the prefab of our weapon item

    private void OnEnable()
    {
        CreateWeapon();
    }

    // instantiate our weapons
    void CreateWeapon()
    {
        // get our weapon maker and choose a weapon based off of that maker
        List<GameObject> weapons = new List<GameObject>();

        if (WeaponMaker == WeaponMakers.Random)
        {
            WeaponMaker = (WeaponMakers)Random.Range(1, (int)WeaponMakers.end);
        }

        switch (WeaponMaker)
        {
            case WeaponMakers.Valkyrie:
                weapons = ValkyrieWeapons;
                break;

            case WeaponMakers.Hera:
                weapons = HeraWeapons;
                break;

            case WeaponMakers.Athena:
                weapons = AthenaWeapons;
                break;

            case WeaponMakers.Deimos:
                weapons = DeimosWeapons;
                break;

            case WeaponMakers.Phobos:
                weapons = PhobosWeapons;
                break;
        }
   
        // then instantiate our weapon object
        weaponObject = weapons[Random.Range(0,weapons.Count)];

        // then create the weapon item
        CreateWeaponItem();
    }

    // instantiate our weapon item
    void CreateWeaponItem()
    {
        // instantiate a copy of the weapon we are currently holding
        Weapon_Item copyItem = Instantiate(weaponItem, transform.position, Quaternion.identity).GetComponent<Weapon_Item>();
        GameObject copyWeapon = Instantiate(weaponObject, Vector3.zero, Quaternion.identity);
        copyWeapon.SetActive(false);
        copyItem.weapon = copyWeapon;
        Destroy(weaponObject);
    }
}
