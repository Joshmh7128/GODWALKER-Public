using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineScript : MonoBehaviour
{
    /// manages mines that the player can drop
    /// 

    [SerializeField] float radius; // how large is this bomb?
    [SerializeField] GameObject explosionPrefab; // the player's explosion prefab
    [SerializeField] Collider localCollider; // our mesh collider

    // instances
    ArenaManager arenaManager; ArenaHandler currentArena;
    PlayerWeaponManager weaponManager;
    private void Awake()
    {
        arenaManager = ArenaManager.instance;
        try { currentArena = arenaManager.activeArena; } catch { Debug.LogWarning("No activeArena found. Ignore if this is displayed in a non-combat area."); }
        weaponManager = PlayerWeaponManager.instance;
    }

    // check every frame to see if we explode
    private void FixedUpdate() => MineCheck();


    // check to see if any of the enemies walk into our mines using a distance loop
    void MineCheck()
    {
        // run an overlap sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        // loop through the colliders
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                MineExplode(collider.transform);
            }
        }
    }

    void MineExplode(Transform enemy)
    {
        // spawn an explosion at the enemy
        PlayerExplosionScript explosionA = Instantiate(explosionPrefab, enemy.position, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
        explosionA.damage = weaponManager.currentWeapon.damage * Random.Range(0.9f, 1.25f);
        // spawn an explosion at our position
        PlayerExplosionScript explosionB = Instantiate(explosionPrefab, transform.position, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
        explosionB.damage = weaponManager.currentWeapon.damage * Random.Range(0.9f, 1.25f);
        // add rage
        PlayerRageManager.instance.AddRage(gameObject.GetComponent<PlayerProjectileScript>().rageAdd);

        // destroy this object
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
