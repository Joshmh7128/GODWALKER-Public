using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalChallengeClass : ChallengeHandler
{
    /// <summary>
    /// A survival challenge is a challenge where enemies spawn over a period of time.
    /// Once the timer is up, all enemies die and the challenge ends
    /// To facilitate this, on activation begin spawning one enemy at an interval of totalTime/enemyCount
    /// </summary>
    /// 
    [Header("Enemy Pool")]
    [SerializeField] List<GameObject> enemies; // the enemies we will spawn overtime
    [SerializeField] List<Transform> groundSpawnPoints, flyingSpawnPoints; // all the spawnpoints we can use
    [SerializeField] float survivalTimeMax, survivalTimeRemaining; // define our gameplay objectives
    [SerializeField] float spawnRate, spawnRateMax, spawnIndex, spawnRateDecreasePercent; // automatically determined
    [SerializeField] Text holdToStartText; // text that tells the player how to start, deactivate on-activate
    [SerializeField] GameObject summoningParticle; // the summoning particle
    [SerializeField] List<GameObject> particles = new List<GameObject>(); // our particles to be destroyed

    // start runs when the object is active in the scene
    public override void PassStart()
    {
        Debug.Log("start run");
        // ENSURE WE ALWAYS HAVE A SPAWN RATE
        if (spawnRateMax == 0) { spawnRateMax = 3; Debug.LogError("Spawn rate was not set in survival challenge - autoset to 3"); }
    }

    // activate the zone
    public override void Activate()
    {
        // debug
        Debug.Log("Challenge Activated");
        // setup our survival time
        survivalTimeRemaining = survivalTimeMax;
        // start our countdown
        StartCoroutine(Countdown());
        // set our spawn interval
        spawnRate = spawnRateMax;
        // start spawning enemies
        StartCoroutine(SpawnEnemy());
        // hide our text
        holdToStartText.text = "";
        // set our bubble target size
        bubbleTargetSize = bubbleMaxSize;
        // trigger the popup dialogue on our player character
        StartCoroutine(InteractionDisplayActivate());
        // do a music request
        if (FindObjectOfType<MusicController>() != null)
        {
            FindObjectOfType<MusicController>().MusicMood(MusicController.musicMoods.battle);
        }

        // spawn summoning particles at every spawnpoint
        foreach (Transform spawnpoint in groundSpawnPoints)
        {
            GameObject particle = Instantiate(summoningParticle, spawnpoint);
            particles.Add(particle);
        }

        foreach (Transform spawnpoint in flyingSpawnPoints)
        {
            GameObject particle = Instantiate(summoningParticle, spawnpoint);
            particles.Add(particle);
        }

        // lock our doors
        foreach (DoorClass door in doorClasses)
        {
            door.Lock();
        }
    }

    // our countdown timer
    IEnumerator Countdown()
    {
        // wait one second
        yield return new WaitForSeconds(1f);

        // if we run out of time, end it
        if (survivalTimeRemaining <= 0)
        {
            EndChallenge();
        }

        // if we have time left, countdown more
        if (survivalTimeRemaining > 0)
        {
            survivalTimeRemaining--;
            yield return new WaitForSeconds(1f);
            StartCoroutine(Countdown());
            UpdateInfo(optionalInfo: "");
        }
    }

    // spawn enemies at the pre-defined interval throughout our spawnpoints
    IEnumerator SpawnEnemy()
    {
        // raise our spawn index
        spawnIndex = Random.Range(0, enemies.Count);
        // debug
        Debug.Log("Spawning enemy...");
        Debug.Log("Waiting...");
        // wait for the spawn
        yield return new WaitForSeconds(spawnRate);
        // what kind of enemy are we spawning?
        if (complete == false)
        {
            if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.ground)
            {
                Debug.Log("Spawning ground enemy...");

                // spawn at the ground spawn points
                GameObject enemy = Instantiate(enemies[(int)spawnIndex], groundSpawnPoints[Random.Range(0, groundSpawnPoints.Count)].position, Quaternion.identity);
                activeEnemies.Add(enemy);
                Debug.Log("Spawned ground enemy");

            }
            else if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.flying)
            {
                if (flyingSpawnPoints.Count > 0)
                {
                    // spawn at the ground spawn points
                    GameObject enemy = Instantiate(enemies[(int)spawnIndex], flyingSpawnPoints[Random.Range(0, flyingSpawnPoints.Count)].position, Quaternion.identity);
                    activeEnemies.Add(enemy);
                    Debug.Log("Spawned flying enemy");
                }
                else
                {
                    Debug.LogWarning("No Flying Spawnpoints Set!");
                }
            }

            // speed up our spawn rate by 5% every time we spawn so that calamity ensues
            if (spawnRate > 0.75f)
            {
                spawnRate = spawnRate - spawnRate * spawnRateDecreasePercent;
            }

            // restart
            StartCoroutine(SpawnEnemy());
        }
    }

    // update our information on the panel
    public override void UpdateInfo(string optionalInfo)
    {
        // since this is a survival challenge, show the seconds remaining
        infoText.text = "Survive for: " + survivalTimeRemaining + " seconds.";
    }

    public override void EndChallenge()
    {
        // when our challenge is over, kill all the enemies
        foreach (GameObject enemy in activeEnemies)
        {
            // kill that enemy
            if (enemy != null)
            enemy.GetComponent<EnemyClass>().OnDeath(); // right now all enemies have OnDeath functions, list is gameobjects so we can mod it later
        }

        // hide our text
        holdToStartText.text = "Challenge Complete";
        infoText.text = "Challenge Complete";

        // shrink the bubble
        bubbleTargetSize = 0f;

        // turn off its collider
        bubbleCollider.SetActive(false);

        // show the challenge is over
        StartCoroutine(InteractionDisplayComplete());

        // do a music request
        if (FindObjectOfType<MusicController>() != null)
        {
            FindObjectOfType<MusicController>().MusicMood(MusicController.musicMoods.explore);
        }

        // remove particles
        foreach (GameObject particle in particles)
        {
            Destroy(particle);
        }

        // unlock all doors associated to this room
        foreach (DoorClass door in doorClasses)
        {
            door.Unlock();
        }

        // set our lights correctly
        combatLightParent.SetActive(false);
        safeLightParent.SetActive(true);

        // unlock our doors
        foreach (DoorClass door in doorClasses)
        {
            door.Unlock();
        }

        // we did it
        complete = true;
    }

    IEnumerator InteractionDisplayComplete()
    {
        // show the challenge is over
        UpgradeSingleton.Instance.player.InteractableMessageTrigger("Challenge Complete", true);
        // wait
        yield return new WaitForSeconds(3f);
        // remove dialogue
        UpgradeSingleton.Instance.player.InteractableMessageTrigger("Challenge Complete", false);
    }

    IEnumerator InteractionDisplayActivate()
    {
        // show the challenge is over
        UpgradeSingleton.Instance.player.InteractableMessageTrigger("Challenge Activated", true);
        // wait
        yield return new WaitForSeconds(3f);
        // remove dialogue
        UpgradeSingleton.Instance.player.InteractableMessageTrigger("Challenge Activated", false);
    }
}
