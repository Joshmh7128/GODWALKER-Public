using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using JetBrains.Annotations;

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
    public bool combatBegun, combatComplete;

    // our list of spawn points
    public List<Transform> spawnPoints = new List<Transform>(); // all the spawnpoints in the room
    Transform spawnPoint; // the spawn point we're using right now

    // our doors
    [HideInInspector] public List<DoorScript> doors;

    // arena manager
    ArenaManager arenaManager;

    // our arena level
    public int arenaLevel;

    // everything to do with upgrades
    [SerializeField] Transform upgradeSpawnPoint; // where the upgrade spawns
    [SerializeField] GameObject bodyPartItem; // an empty body part item prefab
    [SerializeField] bool specialRoom; // is this a special room?

    private void Start()
    {
        // build an arena from our geometry prefabs
        BuildArena();
        // get our arena manager instance
        arenaManager = ArenaManager.instance;
    }

    // select a random geomety set and spawn it in
    void BuildArena()
    { 
        // lock the doors
        foreach (DoorScript door in doors)
        {
            // door.canOpen = false;
            door.triggerLock = true;
            // activate the barriers if they are open
            if (door.open)
                door.Lock();
        }

        // nav mesh generation is done PER MESH inside the geometry prefab
    }

    private void FixedUpdate()
    {
        if (combatBegun)
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
        child.parent = activeParent; // set the parent
        child.transform.position = spawnPoint.position; // set the position
        child.gameObject.GetComponent<EnemyClass>().level = arenaLevel; // set the level of the enemy to the level of the arena
        child.gameObject.SetActive(true); // turn on the enemy
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

    // start combat here - called from the associated doorclass
    public void StartCombat()
    {
        combatBegun = true;
        // set ourselves as the active arena in the arena manager
        arenaManager.activeArena = this;
    }

    // end combat here
    void EndCombat()
    {
        // end combat
        if (!combatComplete)
        {
            combatComplete = true;
            CheckCombatCompletion();
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.outro);

            // spawn our new body part from the list
            // 50/50 chance to get the next in the same set
            CreateBodyPartItem(specialRoom);

        }

    }

    // create a body part set
    void CreateBodyPartItem(bool special)
    {
        // if we are not special
        if (!special)
        {
            // 50/50 chance to generate main or alternate
            int c = Random.Range(0, 100); 
            // spawn a main bodypart at the upgade spawn point
            if (c < 50)
            { 
                // if we havent spawned all items yet
                if (arenaManager.mainIndex <= arenaManager.mainSet.Count)
                {
                    // spawn in a body part item and then add the associated upgrade to that item
                    BodyPart_Item item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPart_Item>();
                    // set the bodypart
                    item.bodyPartObject = Instantiate(arenaManager.mainSet[arenaManager.mainIndex], new Vector3(9999, 9999, 9999), Quaternion.identity);
                    // then add one to the main index so we get another one next
                    arenaManager.mainIndex++;
                }

                // if we have then spawn a special 
                // SpawnSpecialItem();

            } 
            
            // spawn an alternate bodypart at the upgade spawn point
            if (c >= 50)
            {
                // if we havent spawned all items yet
                if (arenaManager.alternateIndex <= arenaManager.alternateSet.Count)
                {
                    BodyPart_Item item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPart_Item>();

                    item.bodyPartObject = Instantiate(arenaManager.alternateSet[arenaManager.alternateIndex], new Vector3(9999, 9999, 9999), Quaternion.identity);
                    // then add one to the main index so we get another one next
                    arenaManager.alternateIndex++;
                }

                // if we have then spawn a special 
                // SpawnSpecialItem();
            }
        }

        // spawn a special
        if (special) SpawnSpecialItem();
    }

    void SpawnSpecialItem()
    {
        BodyPart_Item item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPart_Item>();
        int i = Random.Range(0, arenaManager.specialSet.Count);
        item.bodyPartObject = Instantiate(arenaManager.specialSet[i], new Vector3(9999, 9999, 9999), Quaternion.identity);

        // then remove the part from the list so we cant spawn it again
        arenaManager.specialSet.Remove(arenaManager.specialSet[i]);
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
