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
    [SerializeField] List<GameObject> WeaponsToSpawn = new List<GameObject>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform> ();

    // our weapon objects
    [SerializeField] GameObject weaponItem; // the prefab of our weapon item
    Weapon_Item copyItem;
    WeaponClass weaponClass;


    private static System.Random rng = new System.Random();

    private void Start()
    {
        CreateWeapons();
    }

    // instantiate our weapons
    void CreateWeapons()
    {
        // randomly place our weapons around the map
        int n = spawnPoints.Count;

        // shuffle spawnpoints
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Transform value = spawnPoints[k];
            spawnPoints[k] = spawnPoints[n];
            spawnPoints[n] = value;
        }

        // now randomly place the weapons
        for(int i = 0; i < WeaponsToSpawn.Count; i++)
        {
            try { CreateWeaponItem(WeaponsToSpawn[i],spawnPoints[i]); } catch { }
        }
    }

    // instantiate our weapon item
    void CreateWeaponItem(GameObject weaponObject, Transform spawnPoint)
    {
        // instantiate a copy of the weapon we are currently holding
        copyItem = Instantiate(weaponItem, spawnPoint.position, Quaternion.identity).GetComponent<Weapon_Item>();
        // create a copy of the weaponObject we are holding so that we can give it to the player
        copyItem.weapon = Instantiate(weaponObject, Vector3.zero, Quaternion.identity);
        copyItem.weapon.GetComponent<WeaponClass>().level = level;
        // undo damage calculation so it can be redone
        copyItem.weapon.GetComponent<WeaponClass>().UpdateStats();
        copyItem.weapon.GetComponent<WeaponClass>().updated = true;

        copyItem.weapon.SetActive(false);
        weaponClass = copyItem.weapon.GetComponent<WeaponClass>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            DebugSpawn();
        }
    }

    // debug spawn a weapon in near the player
    void DebugSpawn()
    {
        CreateWeaponItem(WeaponsToSpawn[Random.Range(0, WeaponsToSpawn.Count)], PlayerController.instance.transform);
    }
}
