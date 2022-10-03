using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineScript : MonoBehaviour
{
    /// manages mines that the player can drop
    /// 

    [SerializeField] float radius; // how large is this bomb?
    [SerializeField] GameObject explosionPrefab; // the player's explosion prefab
    [SerializeField] Collider collider; // our mesh collider
    int i = 0; // counter for activity

    // instances
    ArenaManager arenaManager; ArenaHandler currentArena;
    private void Awake()
    {
        arenaManager = ArenaManager.instance;
        currentArena = arenaManager.activeArena;
    }

    // check every frame to see if we explode
    private void FixedUpdate() => MineCheck();

    // check to see if any of the enemies walk into our mines using a distance loop
    void MineCheck()
    {
        if (i < 5) i++;
        if (i >= 5) { collider.enabled = true; }

        foreach(Transform enemy in currentArena.activeParent)
        {
            if (Vector3.Distance(transform.position, enemy.position) < radius)
            {
                // blow up that enemy
                MineExplode(enemy);
            }
        }
    }

    void MineExplode(Transform enemy)
    {
        // spawn an explosion at the enemy
        Instantiate(explosionPrefab, enemy.position, Quaternion.identity, null);
        // spawn an explosion at our position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        // destroy this object
        Destroy(gameObject);
    }
}
