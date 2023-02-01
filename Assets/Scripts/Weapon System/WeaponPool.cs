using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPool : MonoBehaviour
{
    /// this script holds a series of weapons that can be spawned
    /// it is attached the player so that different weapons creators can pull from its list and spawn new weapons after each encounter
    /// 

    // this is a list of all the weapon objects (NOT ITEMS) to be spawned
    public List<GameObject> AllGameWeapons = new List<GameObject>(); // it is a list of all weapons in the game
    public List<GameObject> DiscoveredWeapons = new List<GameObject>(); // this is a list of all weapons which the player has discovered
    public List<GameObject> UndiscoveredWeapons = new List<GameObject>(); // this is a list of all weapons which the player has undiscovered

    // this is a default weapon item
    [SerializeField] GameObject weaponItem;
    // this is an insance copy item which is reused per weapon spawn
    Weapon_Item copyItem;

    public static WeaponPool instance; // our instance
    private void Awake()
    {
        instance = this;
    }

    // start runs when this object comes alive
    private void Start()
    {
        
    }

    // setup our weapon list based off of our save data, located on the player's SaveDataHandler instance
    public void SetupWeaponList()
    {

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
    
    // instantiate our weapon item with our selected weapon object at a specific position
    public GameObject CreateWeaponItem(GameObject weaponObject, Transform spawnPoint, Transform parent)
    {
        // instantiate a copy of the weapon we are currently holding
        copyItem = Instantiate(weaponItem, spawnPoint.position, Quaternion.identity).GetComponent<Weapon_Item>();
        // create a copy of the weaponObject we are holding so that we can give it to the player
        copyItem.weapon = Instantiate(weaponObject, Vector3.zero, Quaternion.identity);
        // undo damage calculation so it can be redone
        copyItem.weapon.GetComponent<WeaponClass>().UpdateStats();
        copyItem.weapon.GetComponent<WeaponClass>().updated = true;

        copyItem.weapon.SetActive(false);

        // set the parent of this object
        copyItem.transform.parent = parent;

        return copyItem.gameObject;
    }

}
