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

    [SerializeField] List<GameObject> enemies; // the enemies we will spawn overtime
    [SerializeField] List<Transform> groundSpawnPoints, flyingSpawnPoints; // all the spawnpoints we can use
    [SerializeField] float survivalTimeMax, survivalTimeRemaining; // define our gameplay objectives
    float spawnRate, spawnIndex; // automatically determined
    [SerializeField] Text holdToStartText; // text that tells the player how to start, deactivate on-activate

    // start runs when the object is active in the scene
    private void Start()
    {

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
        spawnRate = survivalTimeMax / (enemies.Count);
        // start spawning enemies
        StartCoroutine(SpawnEnemy());
        // hide our text
        holdToStartText.text = "";
        // set our bubble target size
        bubbleTargetSize = bubbleMaxSize;
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
        // debug
        Debug.Log("Spawning enemy...");
        Debug.Log("Waiting...");
        // wait for the spawn
        yield return new WaitForSeconds(spawnRate);
        Debug.Log("Waiting Done");
        // what kind of enemy are we spawning?
        if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.ground)
        {
            if (groundSpawnPoints.Count > 0)
            {
                // spawn at the ground spawn points
                Instantiate(enemies[(int)spawnIndex], groundSpawnPoints[Random.Range(0, groundSpawnPoints.Count)].position, Quaternion.identity);
                Debug.Log("Spawned ground enemy");
            } else
            {
                Debug.LogWarning("No Ground Spawnpoints Set!");
            }
        }

        if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.flying)
        {
            if (flyingSpawnPoints.Count > 0)
            {
                // spawn at the ground spawn points
                Instantiate(enemies[(int)spawnIndex], flyingSpawnPoints[Random.Range(0, flyingSpawnPoints.Count)].position, Quaternion.identity);
                Debug.Log("Spawned flying enemy");
            } else
            {
                Debug.LogWarning("No Flying Spawnpoints Set!");
            }
        }


        // spawn one at a spawn point
        // restart
        StartCoroutine(SpawnEnemy());
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
            enemy.GetComponent<EnemyClass>().OnDeath(); // right now all enemies have OnDeath functions, list is gameobjects so we can mod it later
        }

        // hide our text
        holdToStartText.text = "Challenge Complete";

        // shrink the bubble
        bubbleTargetSize = 0f;
    }
}
