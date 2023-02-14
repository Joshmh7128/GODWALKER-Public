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
    float i = 0; // counter for activity

    // instances
    ArenaManager arenaManager; ArenaHandler currentArena;
    PlayerWeaponManager weaponManager;
    private void Awake()
    {
        arenaManager = ArenaManager.instance;
        currentArena = arenaManager.activeArena;
        weaponManager = PlayerWeaponManager.instance;
    }

    // check every frame to see if we explode
    private void FixedUpdate() => MineCheck();

    // check to see if any of the enemies walk into our mines using a distance loop
    void MineCheck()
    {
        if (i < 0.25) i += Time.deltaTime;
        if (i >= 0.25) try { localCollider.enabled = true; } catch { }

        try { 
            foreach(Transform enemy in currentArena.activeParent)
            {
                if (Vector3.Distance(transform.position, enemy.position) < radius)
                {
                    // blow up that enemy
                    MineExplode(enemy);
                }
            }
        } catch { }
    }

    void MineExplode(Transform enemy)
    {
        // spawn an explosion at the enemy
        PlayerExplosionScript explosionA = Instantiate(explosionPrefab, enemy.position, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
        explosionA.damage = weaponManager.currentWeapon.damage * Random.Range(0.9f, 1.25f);
        // spawn an explosion at our position
        PlayerExplosionScript explosionB = Instantiate(explosionPrefab, transform.position, Quaternion.identity, null).GetComponent<PlayerExplosionScript>();
        explosionB.damage = weaponManager.currentWeapon.damage * Random.Range(0.9f, 1.25f);
        // destroy this object
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
