using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPool : MonoBehaviour
{
    /// this script holds a series of weapons that can be spawned
    /// it is attached the player so that different weapons creators can pull from its list and spawn new weapons after each encounter
    /// 

    // this is a list of all the weapon objects (NOT ITEMS) to be spawned
    public List<GameObject> WeaponsToSpawn = new List<GameObject>();

    // this is a default weapon item
    [SerializeField] GameObject weaponItem;
    // this is an insance copy item which is reused per weapon spawn
    Weapon_Item copyItem;

    public static WeaponPool instance; // our instance
    private void Awake()
    {
        instance = this;
    }

    // instantiate our weapon item with our selected weapon object at a specific position
    public void CreateWeaponItem(GameObject weaponObject, Transform spawnPoint)
    {
        // instantiate a copy of the weapon we are currently holding
        copyItem = Instantiate(weaponItem, spawnPoint.position, Quaternion.identity).GetComponent<Weapon_Item>();
        // create a copy of the weaponObject we are holding so that we can give it to the player
        copyItem.weapon = Instantiate(weaponObject, Vector3.zero, Quaternion.identity);
        // undo damage calculation so it can be redone
        copyItem.weapon.GetComponent<WeaponClass>().UpdateStats();
        copyItem.weapon.GetComponent<WeaponClass>().updated = true;

        copyItem.weapon.SetActive(false);
    }

}
