using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolItemRequest : MonoBehaviour
{
    enum PoolChoices { DiscoveredWeaponsForSpawning, UndiscoveredWeaponsForSpawning, DEBUG_AllGameWeapons, DEBUG_DiscoveredWeapons, DEBUG_UndiscoveredWeapons }
    [SerializeField] PoolChoices poolChoice; // our pool to pull from
    [SerializeField] bool spawnOnStart; // should this weapon spawn on start?
    [SerializeField] Transform targetParent;

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
        GameObject weapon = null;

        // run a try because the weapon list may be empty
        try
        {
            WeaponPool pool = WeaponPool.instance;
            switch (poolChoice)
            {
                // normal uses
                case PoolChoices.DiscoveredWeaponsForSpawning:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.DiscoveredWeaponsForSpawning[Random.Range(0, WeaponPool.instance.DiscoveredWeaponsForSpawning.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent);
                    // then remove this item from that list
                    WeaponPool.instance.DiscoveredWeaponsForSpawning.Remove(weapon);
                    break;

                case PoolChoices.UndiscoveredWeaponsForSpawning:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.UndiscoveredWeaponsForSpawning[Random.Range(0, WeaponPool.instance.UndiscoveredWeaponsForSpawning.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent);
                    // then remove this item from that list
                    WeaponPool.instance.UndiscoveredWeaponsForSpawning.Remove(weapon);
                    break;

                // debug uses
                case PoolChoices.DEBUG_AllGameWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.ActivePlayerWeapons[Random.Range(0, WeaponPool.instance.ActivePlayerWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent);
                    break;

                case PoolChoices.DEBUG_DiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.DiscoveredWeapons[Random.Range(0, WeaponPool.instance.DiscoveredWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent);
                    break;

                case PoolChoices.DEBUG_UndiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.UndiscoveredWeapons[Random.Range(0, WeaponPool.instance.UndiscoveredWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, targetParent);
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
