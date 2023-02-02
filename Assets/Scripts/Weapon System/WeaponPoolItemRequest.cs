using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolItemRequest : MonoBehaviour
{
    enum PoolChoices { DiscoveredWeaponsForSpawning, UndiscoveredWeaponsForSpawning, DEBUG_AllGameWeapons, DEBUG_DiscoveredWeapons, DEBUG_UndiscoveredWeapons }
    [SerializeField] PoolChoices poolChoice; // our pool to pull from
    [SerializeField] bool spawnOnStart; // should this weapon spawn on start?

    private void Start()
    {
        if (spawnOnStart) SpawnWeapon();
    }

    // call this function to spawn in a new weapon
    public void SpawnWeapon()
    {
        GameObject weapon = null;
        // run a try because the weapon list may be empty
        try
        {
            switch (poolChoice)
            {
                // normal uses
                case PoolChoices.DiscoveredWeaponsForSpawning:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.DiscoveredWeaponsForSpawning[Random.Range(0, WeaponPool.instance.DiscoveredWeaponsForSpawning.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                    // then remove this item from that list
                    WeaponPool.instance.DiscoveredWeaponsForSpawning.Remove(weapon);
                    break;

                case PoolChoices.UndiscoveredWeaponsForSpawning:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.UndiscoveredWeaponsForSpawning[Random.Range(0, WeaponPool.instance.UndiscoveredWeaponsForSpawning.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                    // then remove this item from that list
                    WeaponPool.instance.UndiscoveredWeaponsForSpawning.Remove(weapon);
                    break;

                // debug uses
                case PoolChoices.DEBUG_AllGameWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.ActivePlayerWeapons[Random.Range(0, WeaponPool.instance.ActivePlayerWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                    break;

                case PoolChoices.DEBUG_DiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.DiscoveredWeapons[Random.Range(0, WeaponPool.instance.DiscoveredWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                    break;

                case PoolChoices.DEBUG_UndiscoveredWeapons:
                    // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                    weapon = WeaponPool.instance.UndiscoveredWeapons[Random.Range(0, WeaponPool.instance.UndiscoveredWeapons.Count)];
                    WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                    break;

            }
        }
        catch
        {
            Debug.LogWarning("Weapon failed to spawn. This may be due to an empty ForSpawning weapon list.");
        }
    }

}
