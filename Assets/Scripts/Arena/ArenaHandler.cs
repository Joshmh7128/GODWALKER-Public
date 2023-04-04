using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaHandler : MonoBehaviour
{
    /// <summary>
    /// This script manages one combat arena
    /// It enables enemies in order from it's inactive pool, and continues until there are none left
    /// then it spawns a reward (?)
    /// </summary>
    /// 

    public Transform activeParent, inactiveParent, doorParent, safetySpawn, setParent; // our parents for inactive and active enemies, door parent is deactivated at thened of combat
    [SerializeField] GameObject summoningEffect; // the visual effect for where an enemy will be summoned
    GameObject previousSummon; // our previous summon
    [SerializeField] int activeGoal; // how many do we want active at once?
    public bool combatBegun, combatComplete;

    // our list of spawn points
    public Transform spawnPointParent;
    public List<Transform> spawnPoints = new List<Transform>(); // all the spawnpoints in the room

    // our list of waves
    [SerializeField] Transform masterWave; // the master wave parent
    [SerializeField] List<Transform> waveParents; // wave parents hold enemies as child objects

    // our doors
    [HideInInspector] public List<DoorScript> doors;

    // instances
    ArenaManager arenaManager;
    PlayerRageManager playerRageManager;

    // our visual fx
    [SerializeField] List<GameObject> rendererObjects;

    // our arena level
    public int arenaLevel;
    [SerializeField] float waveWaitTime; // the wait time of waves

    // everything to do with upgrades
    [SerializeField] Transform upgradeSpawnPoint; // where the upgrade spawns
    [SerializeField] bool specialRoom; // is this a special room?
    public bool manualCombat; // manual editor combat start

    [HeaderAttribute("Generative Enemies")]
    // everything to do with generative combat
    public bool spawnWavesFromData; // are we creating waves from data?
    public int currentRoom; // which room are we in? 
    public int currentEncounter; // which encounter?
    // we need our scriptable object
    EnemySetObject encounterSetObject;

    PlayerGenerationSeedManager seedManager;

    public enum ArenaModes
    {
        Wave, // wave based combat
        Single, // one single combat
        GoalAmount // keep as many enemies in the environment as you can
    }

    public ArenaModes arenaMode; 

    private void Start()
    {
        // before we do anything, get our encounter set object
        encounterSetObject = Resources.Load<EnemySetObject>("EncounterSets");

        // get our arena manager instance
        arenaManager = ArenaManager.instance;
        playerRageManager = PlayerRageManager.instance;
        seedManager = PlayerGenerationSeedManager.instance;

        // build an arena from our geometry prefabs
        BuildArena();
    }

    // put enemies in room
    void BuildArena()
    {
        // set our current room
        currentRoom = seedManager.currentCombatPos; // get our player's current position in the run

        // before we build our arena, we need to build our waves
        if (spawnWavesFromData)
        {
            // get the room from our data and get the info from the encounter set
            // instantiate each wave from our selected encounter
            // foreach wave in our wave list
            int w = 0; // our current wave
            foreach (Wave wave in encounterSetObject.roomList[currentRoom].encounterList[currentEncounter].waveList) // right now currentEncounter is set to 0 because we only have one encounter per room
            {
                // advance the wave we're on for aesthetic display
                w++;
                // spawn the wave under out master wave, then spawn all the enemies of that wave underneath it
                GameObject newWave = Instantiate(new GameObject(), masterWave); newWave.name = "Wave " + w;

                // then foreach enemy in the wave's enemy list instantiate it in as with newWave as its parent
                foreach(GameObject enemy in wave.enemyList)
                {
                    // make sure to spawn in the enemy and ENSURUE IT IS INACTIVE
                    Instantiate(enemy, newWave.transform).SetActive(false);
 
                }

                // check and set its elemental protection. the enemyClass automatically will turn on its shields on activation
                // because we are one ahead by the time the player starts combat, we need to check the run pos from when we entered the room
                int location = seedManager.currentRunPos - 1;

                // half energy shielded
                if (seedManager.elementBiases[location] == PlayerGenerationSeedManager.ElementBiases.partialEnergy)
                {
                    // put all the children in a list and shuffle
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in newWave.transform)
                        children.Add(child.gameObject);

                    children.Shuffle();

                    // we want half to be shielded
                    for (int z = 0; z < children.Count / 2; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.energyShield;
                    }
                }

                // half explosive shielded
                if (seedManager.elementBiases[location] == PlayerGenerationSeedManager.ElementBiases.partialExplosive)
                {
                    // put all the children in a list and shuffle
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in newWave.transform)
                        children.Add(child.gameObject);

                    children.Shuffle();

                    // we want half to be shielded
                    for (int z = 0; z < children.Count / 2; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.explosiveShield;
                    }
                }

                // check for 50/50 mixed assignments
                if (seedManager.elementBiases[location] == PlayerGenerationSeedManager.ElementBiases.partialMixed)
                {
                    // put all the children in a list and shuffle
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in newWave.transform)
                        children.Add(child.gameObject);

                    children.Shuffle();
                    int z;
                    // we want half to be shielded
                    for (z = 0; z < children.Count / 2; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.energyShield;
                    }

                    // we want half to be shielded
                    for (z = z; z < children.Count; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.explosiveShield;
                    }
                }

                // all energy shielded
                if (seedManager.elementBiases[location] == PlayerGenerationSeedManager.ElementBiases.allEnergy)
                {
                    // put all the children in a list and shuffle
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in newWave.transform)
                        children.Add(child.gameObject);

                    children.Shuffle();

                    // we want half to be shielded
                    for (int z = 0; z < children.Count; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.energyShield;
                    }
                }

                // all explosive shielded
                if (seedManager.elementBiases[location] == PlayerGenerationSeedManager.ElementBiases.allExplosive)
                {
                    // put all the children in a list and shuffle
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in newWave.transform)
                        children.Add(child.gameObject);

                    children.Shuffle();

                    // we want half to be shielded
                    for (int z = 0; z < children.Count; z++)
                    {
                        children[z].GetComponent<EnemyClass>().activeElementalProtection = EnemyClass.ElementalProtection.explosiveShield;
                    }
                }

            }

        }

        if (spawnPointParent)
        {
            foreach(Transform spawnPoint in spawnPointParent)
            {
                spawnPoints.Add(spawnPoint);
            }
        }

        // get the waves
        foreach (Transform childwave in masterWave)
        {
            waveParents.Add(childwave);
        }

        // lock the doors
        foreach (DoorScript door in doors)
        {
            // activate the barriers if they are open
            if (door.open)
                door.Lock();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9) && arenaManager.activeArena == this)
        {
            foreach(Transform enemy in activeParent)
            {
                enemy.gameObject.GetComponent<EnemyClass>().GetHurt(999999f, EnemyClass.ElementalProtection.none);
            }
        }

        if (Input.GetKeyDown(KeyCode.F10) && arenaManager.activeArena == this)
        {
            //  try { PlayerController.instance.Teleport(safetySpawn.position); } catch { PlayerController.instance.Teleport(transform.position);  }
        }

    }

    private void FixedUpdate()
    {
        if (manualCombat)
        {
            StartCombat();
            manualCombat = false;
        }

        if (combatBegun)
        {
            if (arenaMode == ArenaModes.GoalAmount)
            ProcessGoalAmount();

            if (arenaMode == ArenaModes.Wave)
            {
                ProcessWave();
            }
        }

        ProcessGlow();

    }

    bool setupWaveRunning; // is setup wave running
    // process our waves
    void ProcessWave()
    {
        // message group alpha handling
        if (messageGroup.alpha < messageGroupTargetA)
        {
            messageGroup.alpha += 2 * Time.deltaTime;
        }

        if (messageGroup.alpha > messageGroupTargetA)
        {
            messageGroup.alpha -= 2 * Time.deltaTime;
        }

        // if there are no enemies in our active parent,
        // get all the child objects of the next wave and make them children of the active parent,
        // then delete the wave parent

        if (activeParent.childCount <= 0 && !setupWaveRunning && waveParents.Count > 0)
        {
            StartCoroutine(SetupWave(false));
        }
        
        if (activeParent.childCount <= 0 && waveParents.Count == 0)
        {
            EndCombat();
        }
    }

    // the coroutine we run to setup a new wave of enemies
    IEnumerator SetupWave(bool instant)
    {
        setupWaveRunning = true;
        List<Transform> vfx = new List<Transform>();
        // if it's not instant, wait
        if (!instant)
        {
            // spawn a bunch of summon vfx
            foreach (Transform child in waveParents[0])
            {
                Transform t = Instantiate(summoningEffect, child.position, Quaternion.identity, null).transform;
                vfx.Add(t);
            }

            if (waveParents.Count > 1)
            {
                StartCoroutine(ShowWaveMessage("Enemies Incoming"));
            }
            else if (waveParents.Count == 1)
            {
                StartCoroutine(ShowWaveMessage("Final Wave Incoming"));
            }

            yield return new WaitForSecondsRealtime(waveWaitTime);
            // then destroy all the vfx when we continue
            foreach (Transform t in vfx)
            {
                Destroy(t.gameObject);
            }
        }
        // if we have a new wave to do
        if (waveParents[0] != null)
        {
            // put all desired enemies into a list
            List<Transform> enemyTransforms = new List<Transform>();
            foreach (Transform child in waveParents[0])
            {
                enemyTransforms.Add(child);
            }

            // setup a list of used spawnpoints for this wave only, so that 
            List<EnemySpawnPoint> usedSpawnPoints = new List<EnemySpawnPoint>(); // we can't reuse these points

            // use the list to move all our enemies over to the correct parent
            foreach (Transform child in enemyTransforms)
            {
                // get enemy type
                EnemyClass.SpawnPointRequirements requirement = child.GetComponent<EnemyClass>().spawnPointRequirement;
                // move it to a spawn point from the relevant type
                // randomly choose a spawnpoint
                int s = Random.Range(0, spawnPoints.Count);
                // store that spawn point
                EnemySpawnPoint eSpawn = spawnPoints[s].gameObject.GetComponent<EnemySpawnPoint>();

                int i = 0; // a counter safety for our while loop
                int j = 0; // loop safety breaker
                // run a while loop until we find a matching spawnpoint, or until we run out of spawn points
                // use a spawn point that fulfills out requirement and hasn't been used
                while ((eSpawn.spawnPointFulfillment != requirement || usedSpawnPoints.Contains(eSpawn) || i < spawnPoints.Count)) 
                {
                    // add to i
                    i++;
                    // edge case exceptions
                    if (i >= spawnPoints.Count)
                    {
                        // if we can't find a far point, set it to a random point
                        if (requirement == EnemyClass.SpawnPointRequirements.groundFarFromPlayer || requirement == EnemyClass.SpawnPointRequirements.centralGrounded)
                            requirement = EnemyClass.SpawnPointRequirements.groundRandom;

                        if (requirement == EnemyClass.SpawnPointRequirements.airFarFromPlayer || requirement == EnemyClass.SpawnPointRequirements.centralAir)
                            requirement = EnemyClass.SpawnPointRequirements.airRandom;

                        // set i to 0 then restart the loop
                        i = 0;
                        
                        // safety breaker
                        j++;
                        if (j > 3)
                        { break; } // if we check all the points and STILL can't do anything, break the loop and spawn the enemy at the most recently chosen point. 
                    }

                    // randomize S
                    s = Random.Range(0, spawnPoints.Count);
                    // change our target spawn point
                    eSpawn = spawnPoints[s].gameObject.GetComponent<EnemySpawnPoint>();
                    // if it fulfills out requirement, break out of the loop
                    if (eSpawn.spawnPointFulfillment == requirement)
                    {
                        break;
                    }
                }

                // now that we have a valid spawn point, move the enemy there 
                child.transform.position = eSpawn.gameObject.transform.position;
                // ensure it is enabled
                child.gameObject.SetActive(true);
                // then set parent
                child.parent = activeParent;
                // add espawn to the list of used spawns for this wave
                usedSpawnPoints.Add(eSpawn);



                if (waveParents[0].childCount <= 0)
                {
                    GameObject parent = waveParents[0].gameObject;
                    waveParents.Remove(parent.transform);
                    Destroy(parent.gameObject);
                }

                // manually start the behaviours
                child.GetComponent<EnemyClass>().ManualBehaviourStart();
            }

            // setup wave has run
            setupWaveRunning = false;
        }
        else
        {
            EndCombat();
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
                //EnableNewEnemyAtSpawnPoint();
            }
        }

        if (activeParent.childCount <= 0 && inactiveParent.childCount <= 0)
        {
            // end the combat
            EndCombat();
        }
    }

    /*
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

    }*/

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
        foreach (Transform transform in activeParent)
        {
            transform.GetComponent<EnemyClass>().ManualBehaviourStart();
        }    
        // run the combat start on our doors
        foreach (DoorScript door in doors)
        {
            door.CombatBegin();
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
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.explore);

            StartCoroutine(ShowWaveMessage("Combat Complete"));

            doorParent.gameObject.SetActive(false);

            // spawn our new body part from the list
            // 50/50 chance to get the next in the same set

        }

    }

    void CreateBodyPartItem(GameObject bodyPart)
    {
        // spawn in a body part item and then add the associated upgrade to that item
        Instantiate(bodyPart, upgradeSpawnPoint.position, Quaternion.identity);
    }

    // check out combat completion
    void CheckCombatCompletion()
    {
        // when we set our combat completion, unlock all our doors
        foreach (DoorScript door in doors)
        {
            door.AttemptUnlock();
        }
    }

    // everything to do with our UI
    [SerializeField] CanvasGroup messageGroup; // our UI group
    [SerializeField] Text messageText; // the message text
    float messageGroupTargetA; // the target alpha of our message group

    IEnumerator ShowWaveMessage(string message)
    {
        messageText.text = message;
        messageGroupTargetA = 1;
        yield return new WaitForSecondsRealtime(4f);
        messageGroupTargetA = 0;
    }

    // process our glowing
    void ProcessGlow()
    {
        foreach(GameObject renderer in rendererObjects)
        {
            // try material
            try {
                renderer.GetComponent<Renderer>().material.SetColor("_EmissionColor", playerRageManager.rageColors[(int)playerRageManager.rageLevel]);
                renderer.GetComponent<Renderer>().material.color = playerRageManager.rageColors[(int)playerRageManager.rageLevel];
            } catch { }

            // try light
            try
            {
                renderer.GetComponent<Light>().color = playerRageManager.rageColors[(int)playerRageManager.rageLevel];
            }
            catch { }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 1);
    }

}
