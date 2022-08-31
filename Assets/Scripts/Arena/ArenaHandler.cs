using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class ArenaHandler : MonoBehaviour
{
    /// <summary>
    /// This script manages one combat arena
    /// It enables enemies in order from it's inactive pool, and continues until there are none left
    /// then it spawns a reward (?)
    /// </summary>
    /// 

    public Transform activeParent, inactiveParent; // our parents for inactive and active enemies
    [SerializeField] GameObject summoningEffect; // the visual effect for where an enemy will be summoned
    GameObject previousSummon; // our previous summon
    [SerializeField] int activeGoal; // how many do we want active at once?
    bool combatComplete;

    // our list of spawn points
    public List<Transform> spawnPoints = new List<Transform>(); // all the spawnpoints in the room
    Transform spawnPoint; // the spawn point we're using right now

    // our list of arena fillers
    [SerializeField] List<GameObject> arenaGeometries; // all the different geometries of arenas we can fill the environment with
    // our doors
    [SerializeField] List<DoorScript> doors;

    private void Start()
    {
        // build an arena from our geometry prefabs
        BuildArena();
    }

    // select a random geomety set and spawn it in
    void BuildArena()
    {
        // build the arenas
        int i = Random.Range(0, arenaGeometries.Count);
        GameObject geometry = Instantiate(arenaGeometries[i], transform.position, Quaternion.identity, null);
        // set this as the geometry's arena, then send the spawnpoints to us
        geometry.GetComponent<ArenaGeometryClass>().handler = this;
        geometry.GetComponent<ArenaGeometryClass>().SendSpawnPoints();
        // lock the doors
        foreach (DoorScript door in doors)
        {
            door.canOpen = false;
            door.triggerLock = true;
            // activate the barriers if they are open
            if (door.open)
                door.ManualLock();
        }

        // nav mesh generation is done PER MESH inside the geometry prefab
    }

    private void FixedUpdate()
    {
        ProcessEnemyAmount();
    }

    // process our enemies
    void ProcessEnemyAmount()
    {
        if (activeParent.childCount < activeGoal)
        {
            if (inactiveParent.childCount > 0)
            if (inactiveParent.GetChild(0) != null)
            {
                EnableNewEnemy();
            }
        }

        if (activeParent.childCount <= 0 && inactiveParent.childCount <= 0)
        {
            // end the combat
            EndCombat();
        }
    }

    void EnableNewEnemy()
    {
        // if our spawnpoint is null, set it to our first one just to get us started
        if (spawnPoint == null) { spawnPoint = spawnPoints[0]; }
        // enable an enemy and move them to a spawn point
        Transform child = inactiveParent.GetChild(0);
        child.parent = activeParent;
        child.transform.position = spawnPoint.position;
        child.gameObject.SetActive(true);
        // set a random spawn point AFTER we spawn this enemy, so that the next one spawns at the same one as the effect
        spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)]; 
        // spawn our summoningEffect where the new enemy will start
        if (previousSummon) { Destroy(previousSummon); }
        if (inactiveParent.childCount > 1)
        previousSummon = Instantiate(summoningEffect, spawnPoint.position, Quaternion.identity, null); // instantiate a new summing effect at the spawn point we have chosen

    }

    public void StopAllEnemyBehaviours()
    {
        foreach (Transform child in activeParent)
        {
            child.gameObject.GetComponent<EnemyClass>().StopAllCoroutines();
        }
    }

    // end combat here
    void EndCombat()
    {
        if (!combatComplete)
        {
            combatComplete = true;
            CheckCombatCompletion();
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.outro);
        }

    }

    // check out combat completion
    void CheckCombatCompletion()
    {
        // when we set our combat completion, unlock all our doors
        foreach (DoorScript door in doors)
        {
            door.Unlock();
        }
    }
}
