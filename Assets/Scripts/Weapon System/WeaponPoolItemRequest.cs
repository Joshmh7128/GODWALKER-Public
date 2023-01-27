using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPoolItemRequest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // as our weapon pool for a random weapon spawn, then remove that weapon from the pool
        GameObject weapon = WeaponPool.instance.WeaponsToSpawn[Random.Range(0, WeaponPool.instance.WeaponsToSpawn.Count)];

        WeaponPool.instance.CreateWeaponItem(weapon, transform, transform);

        // remove the weapon from the weaponstospawn pool
        WeaponPool.instance.WeaponsToSpawn.Remove(weapon);
    }

}
