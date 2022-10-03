using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineScript : MonoBehaviour
{
    /// manages mines that the player can drop
    /// 

    [SerializeField] float radius; // how large is this bomb?
    [SerializeField] GameObject explosionPrefab; // the player's explosion prefab
    [SerializeField] MeshCollider meshCollider; // our mesh collider
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

    private void Start()
    {
        // start our invincible start
        StartCoroutine(InvStart());
    }

    // make the mine inactive at start and then turn on its colliders after 5 frames
    IEnumerator InvStart()
    {
        yield return new WaitForFixedUpdate();
        if (i < 5)
        {
            StartCoroutine(InvStart());
        } else if (i >= 5)
        {
            meshCollider.enabled = true;
        }
    }

    // check to see if any of the enemies walk into our mines using a distance loop
    void MineCheck()
    {
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
