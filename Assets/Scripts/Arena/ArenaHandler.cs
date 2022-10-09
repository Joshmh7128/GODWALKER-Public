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

    // our list of waves
    [SerializeField] List<Transform> waveParents; // wave parents hold enemies as child objects

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

    public enum ArenaModes
    {
        Wave, // wave based combat
        Single, // one single combat
        GoalAmount // keep as many enemies in the environment as you can
    }

    public ArenaModes arenaMode; 

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
    }

    private void FixedUpdate()
    {
        if (combatBegun)
        {
            if (arenaMode == ArenaModes.GoalAmount)
            ProcessGoalAmount();

            if (arenaMode == ArenaModes.Wave)
            {
                ProcessWave();
            }
        }
    }

    // process our waves
    void ProcessWave()
    {
        // if there are no enemies in our active parent,
        // get all the child objects of the next wave and make them children of the active parent,
        // then delete the wave parent

        if (activeParent.childCount <= 0)
        {
            // if we have a new wave to do

            if (waveParents[0] != null)
            {
                // the 0th wave parent
                foreach (Transform child in waveParents[0])
                {
                    // ensure it is enabled
                    child.gameObject.SetActive(true);
                    child.parent = activeParent;

                    if (waveParents[0].childCount <= 0)
                    {
                        GameObject parent = waveParents[0].gameObject;
                        waveParents.Remove(parent.transform);
                        Destroy(parent.gameObject);
                    }

                    // manually start the behaviours
                    child.GetComponent<EnemyClass>().ManualBehaviourStart();
                }
            }
            else
            {
                EndCombat();
            }
        }
    }

    // process our enemies
    void ProcessGoalAmount()
    {
        if (activeParent.childCount < activeGoal)
        {
            if (inactiveParent.childCount > 0)
            if (inactiveParent.GetChild(0) != null)
            {
                EnableNewEnemyAtSpawnPoint();
            }
        }

        if (activeParent.childCount <= 0 && inactiveParent.childCount <= 0)
        {
            // end the combat
            EndCombat();
        }
    }

    void EnableNewEnemyAtSpawnPoint()
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
        // activate all enemies
        foreach(Transform transform in activeParent)
        {
            transform.GetComponent<EnemyClass>().ManualBehaviourStart();
        }    

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
                    BodyPartItem item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPartItem>();
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
                    BodyPartItem item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPartItem>();

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
        BodyPartItem item = Instantiate(bodyPartItem, upgradeSpawnPoint).GetComponent<BodyPartItem>();
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
