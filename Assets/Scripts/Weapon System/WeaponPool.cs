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
        StartCoroutine(LateStartBuffer());
    }

    IEnumerator LateStartBuffer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        LateStart();
    }

    // start that happens 0.5 seconds after the start function
    void LateStart()
    {
        // setup our weapon lists
        SetupWeaponLists();
    }

    // setup our weapon list based off of our save data, located on the player's SaveDataHandler instance
    public void SetupWeaponLists()
    {
        // add all of our default weapons to our discovered list
        foreach (string name in SaveDataHandler.instance.liveData.DefaultWeapons)
        {
            // take the name, look through the AllGameWeapons list, and find the weapons we want to add to it
            for (int i = 0; i < SaveDataHandler.instance.liveData.DefaultWeapons.Count; i++)
            {
                if (AllGameWeapons[i].GetComponent<WeaponClass>().weaponName == name)
                {
                    // only add this object to the list if it does not already contain it
                    if (!DiscoveredWeapons.Contains(AllGameWeapons[i]))
                        DiscoveredWeapons.Add(AllGameWeapons[i]);
                }
            }
        }

        // add all of our discovered weapons to our discovered list
        foreach (string name in SaveDataHandler.instance.liveData.DiscoveredWeapons)
        {
            // take the name and look through all of our DiscoveredWeapons, and find the weapons we want to add to it from the discovered weapons
            for (int i = 0; i < SaveDataHandler.instance.liveData.DiscoveredWeapons.Count; i++)
            {
                if (AllGameWeapons[i].GetComponent<WeaponClass>().weaponName == name)
                {
                    // only add this weapon if it is not in the list yet
                    if (!DiscoveredWeapons.Contains(AllGameWeapons[i]))
                        DiscoveredWeapons.Add(AllGameWeapons[i]);
                }
            }
        }        

        // now add all of our undiscovered weapons to our undiscovered weapons list
        foreach (string name in SaveDataHandler.instance.liveData.UndiscoveredWeapons)
        {
            // take the name from each undiscovered weapon and compare it to the AllGameWeapons list, then add any undiscovered weapons to the undiscovered list
            for (int i = 0; i < SaveDataHandler.instance.liveData.UndiscoveredWeapons.Count; i++)
            {
                if (AllGameWeapons[i].GetComponent<WeaponClass>().weaponName == name)
                {
                    // only add this weapon if it is not in the list yet
                    if (!UndiscoveredWeapons.Contains(AllGameWeapons[i]))
                        UndiscoveredWeapons.Add(AllGameWeapons[i]);
                }
            }
        }

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
