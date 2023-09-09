using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    /// functions as a script to display and show possible enemy spawn points

    // what condition does this spawn point satisfy?
    public EnemyClass.SpawnPointRequirements spawnPointFulfillment;
    [SerializeField] GameObject infoItem; // the item we're using in the editor to display text info
    public bool used; // was this used?
    PlayerController playerController; // our player controller instance
    float farDistance = 50;
    [SerializeField] ArenaHandler arenaHandler;

    // use the start method to trigger a slow tick time for us to recalculate player distances
    private void Start()
    {
        // set our player instance
        playerController = PlayerController.instance;
        // start our custom tick
        StartCoroutine(SlowTick());

        if (arenaHandler == null)
        {
            Debug.LogWarning("Spawnpoint " + name + "has no set ArenaHandler! Was this done on purpose? Setting to nearest Arena... ");
            // check to make sure the spawn point list has us
            if (!FindObjectOfType<ArenaHandler>().spawnPoints.Contains(transform))
                FindObjectOfType<ArenaHandler>().spawnPoints.Add(transform);
        } else if (arenaHandler != null)
        {
            arenaHandler.spawnPoints.Add(transform);
        }
    }

    // a custom slow tick that runs twice per second to do spawn point calculations
    IEnumerator SlowTick()
    {
        // run slow update
        SlowUpdate(); 
        yield return new WaitForSeconds(0.25f);
        // restart
        StartCoroutine(SlowTick());
    }

    // run from our slow tick
    void SlowUpdate()
    {
        // if we are far from the player, set our fulfillment to being far
        if (Vector3.Distance(playerController.transform.position, transform.position) > farDistance)
        {
            // if we were grounded before, set it to far grounded
            if (spawnPointFulfillment == EnemyClass.SpawnPointRequirements.groundRandom)
            {
                spawnPointFulfillment = EnemyClass.SpawnPointRequirements.groundFarFromPlayer;
            }

            // if we were air before, set it to far air
            if (spawnPointFulfillment == EnemyClass.SpawnPointRequirements.airRandom)
            {
                spawnPointFulfillment = EnemyClass.SpawnPointRequirements.airFarFromPlayer;
            }
        }

        // if we are close to the player, set our fulfillment to being close
        if (Vector3.Distance(playerController.transform.position, transform.position) <= farDistance)
        {
            // if we were grounded before, set it to random grounded
            if (spawnPointFulfillment == EnemyClass.SpawnPointRequirements.groundFarFromPlayer)
            {
                spawnPointFulfillment = EnemyClass.SpawnPointRequirements.groundRandom;
            }

            // if we were air before, set it to random air
            if (spawnPointFulfillment == EnemyClass.SpawnPointRequirements.airFarFromPlayer)
            {
                spawnPointFulfillment = EnemyClass.SpawnPointRequirements.airRandom;
            }
        }
    }

    private void OnDrawGizmos()
    {
        try
        {
            switch (spawnPointFulfillment)
            {
                case EnemyClass.SpawnPointRequirements.groundRandom:
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "GR";
                    break;
                case EnemyClass.SpawnPointRequirements.airRandom:
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "AR";
                    break;
                case EnemyClass.SpawnPointRequirements.groundFarFromPlayer:
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "GF";
                    break;
                case EnemyClass.SpawnPointRequirements.airFarFromPlayer:
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "AF";
                    break;
                case EnemyClass.SpawnPointRequirements.centralGrounded:
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "CG";
                    break;
                case EnemyClass.SpawnPointRequirements.centralAir:
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(transform.position, 1f);
                    infoItem.name = "CA";
                    break;
            }
        } catch { }
    }
}
