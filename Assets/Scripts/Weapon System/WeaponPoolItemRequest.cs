using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolItemRequest : MonoBehaviour
{
    enum PoolChoices { AllGameWeapons, DiscoveredWeapons, UndiscoveredWeapons }
    [SerializeField] PoolChoices poolChoice; // our pool to pull from

    // call this function to spawn in a new weapon
    public void SpawnWeapon()
    {
        GameObject weapon = null;
        switch (poolChoice)
        {
            case PoolChoices.AllGameWeapons:
                // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                weapon = WeaponPool.instance.AllGameWeapons[Random.Range(0, WeaponPool.instance.AllGameWeapons.Count)];
                WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                // remove the weapon from the weaponstospawn pool
                WeaponPool.instance.AllGameWeapons.Remove(weapon);
                break;

            case PoolChoices.DiscoveredWeapons:
                // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                weapon = WeaponPool.instance.DiscoveredWeapons[Random.Range(0, WeaponPool.instance.DiscoveredWeapons.Count)];
                WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                // remove the weapon from the weaponstospawn pool
                WeaponPool.instance.DiscoveredWeapons.Remove(weapon);
                break;

            case PoolChoices.UndiscoveredWeapons:
                // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
                weapon = WeaponPool.instance.UndiscoveredWeapons[Random.Range(0, WeaponPool.instance.UndiscoveredWeapons.Count)];
                WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);
                // remove the weapon from the weaponstospawn pool
                WeaponPool.instance.UndiscoveredWeapons.Remove(weapon);
                break;

        }
    }

}
