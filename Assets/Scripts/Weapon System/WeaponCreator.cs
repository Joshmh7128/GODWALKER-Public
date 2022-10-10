using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreator : MonoBehaviour
{
    /// <summary>
    /// This script is used to create weapon items from a table of prefabs
    /// It creates a weapon item with a weapon inside of it
    /// </summary>

    // the level of this weapon
    public float level;

    // the list of weapons per maker
    [SerializeField] List<GameObject> ValkyrieWeapons = new List<GameObject>();

    // our weapon objects
    GameObject weaponObject;
    [SerializeField] GameObject weaponItem; // the prefab of our weapon item
    Weapon_Item copyItem;
    WeaponClass weaponClass;

    private void OnEnable()
    {
        CreateWeapon();
    }

    // instantiate our weapons
    void CreateWeapon()
    {
        // get our weapon maker and choose a weapon based off of that maker
        // then instantiate our weapon object
        weaponObject = ValkyrieWeapons[Random.Range(0, ValkyrieWeapons.Count-1)];

        // then create the weapon item
        CreateWeaponItem();
    }

    // instantiate our weapon item
    void CreateWeaponItem()
    {
        // instantiate a copy of the weapon we are currently holding
        copyItem = Instantiate(weaponItem, transform.position, Quaternion.identity).GetComponent<Weapon_Item>();
        // create a copy of the weaponObject we are holding so that we can give it to the player
        copyItem.weapon = Instantiate(weaponObject, Vector3.zero, Quaternion.identity); 
        copyItem.weapon.SetActive(false);
        weaponClass = copyItem.weapon.GetComponent<WeaponClass>();
        UpdateItem();
    }

    public void UpdateItem()
    {
        weaponClass.level = level; // set our level
        weaponClass.UpdateStats(); // manually update the stats when the item is created
    }
}
