using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolItemRequest : MonoBehaviour
{
    enum PoolChoices { DiscoveredWeaponsForSpawning, UndiscoveredWeaponsForSpawning, DEBUG_AllGameWeapons, DEBUG_DiscoveredWeapons, DEBUG_UndiscoveredWeapons }
    [SerializeField] PoolChoices poolChoice; // our pool to pull from
    [SerializeField] bool spawnOnStart; // should this weapon spawn on start?
    [SerializeField] Transform targetParent;
    public bool discoverOnPickup = false; // should the thing that spawns from here be discovered on pickup?
    [SerializeField] string specificWeapon; // is there a specific weapon we should spawn?

    float maxSpawnAttempts = 30; // after 150 spawn attempts, stop trying.

    private void Start()
    {
        if (targetParent == null)
            targetParent = transform;

        if (spawnOnStart) StartCoroutine(SlowSpawnCheck());
    }

    // run a looping check to make sure we can spawn our weapons using a slow tick time as to not crash the game
    // if spawn weapon fails it runs this coroutine again
    IEnumerator SlowSpawnCheck()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        // if we succeed in this check, spawn the weapon
        SpawnWeapon();
    }

    // call this function to spawn in a new weapon
    public void SpawnWeapon()
    {
        // the weapon we are working with, to be set
        GameObject weapon = null;

        // if this is a debug item then spawn the weapon we want
        if (specificWeapon != "")
        {
            // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
            foreach (GameObject weaponObject in WeaponPool.instance.ActivePlayerWeapons)
            {
                if (weaponObject.GetComponent<WeaponClass>().weaponName == specificWeapon)
                {
                    weapon = weaponObject;
                }
            }

            WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
            return;
        }


        // run a try because the weapon list may be empty
        try
        {
            WeaponPool pool = WeaponPool.instance;
            switch (poolChoice)
            {
                // normal uses
                case PoolChoices.DiscoveredWeaponsForSpawning:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = pool.DiscoveredWeaponsForSpawning[Random.Range(0, pool.DiscoveredWeaponsForSpawning.Count)];
                    pool.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
                    // then remove this item from that list
                    pool.DiscoveredWeaponsForSpawning.Remove(weapon);
                    break;

                case PoolChoices.UndiscoveredWeaponsForSpawning:
                    // check to make sure there are weapons we can discover
                    if (pool.UndiscoveredWeaponsForSpawning.Count > 0)
                    {
                        // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                        weapon = pool.UndiscoveredWeaponsForSpawning[Random.Range(0, pool.UndiscoveredWeaponsForSpawning.Count)];
                        pool.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
                        // then remove this item from that list
                        pool.UndiscoveredWeaponsForSpawning.Remove(weapon);
                    } else // if we do not have any weapons left for spawning, spawn a discovered weapon instead
                    {
                        poolChoice = PoolChoices.DiscoveredWeaponsForSpawning;
                        SpawnWeapon();
                    }
                    break;

                // debug uses
                case PoolChoices.DEBUG_AllGameWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = pool.ActivePlayerWeapons[Random.Range(0, pool.ActivePlayerWeapons.Count)];
                    pool.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
                    break;

                case PoolChoices.DEBUG_DiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = pool.DiscoveredWeapons[Random.Range(0, pool.DiscoveredWeapons.Count)];
                    pool.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
                    break;

                case PoolChoices.DEBUG_UndiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = pool.UndiscoveredWeapons[Random.Range(0, pool.UndiscoveredWeapons.Count)];
                    pool.CreateWeaponItem(weapon, transform, targetParent).GetComponent<Weapon_Item>().discoverOnPickup = discoverOnPickup;
                    break;

            }
        }
        catch
        {
            Debug.LogWarning("Weapon failed to spawn. \n This may be due to an empty ForSpawning weapon list or attempting to spawn weapons before Save Data has finished loading. \n Another spawn attempt will occur now...");
            maxSpawnAttempts--;
            // as our pool to rebuild our lists before we try again
            WeaponPool.instance.BuildWeaponLists();

            // try another spawn
            if (maxSpawnAttempts > 0)
            StartCoroutine(SlowSpawnCheck());
        }
    }

}
