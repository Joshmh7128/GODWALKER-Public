using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] string challengeType, difficultyLevel, reward, fullInfo; // our info strings

    // start runs when the object is active in the scene
    private void Start()
    {
        // set our info text correctly for this challenge
        fullInfo = challengeType + "\n" + difficultyLevel + "\n" + reward;
        // set that to the text on the canvas
        infoText.text = fullInfo;
    }

    // activate the zone
    public override void Activate()
    {
        // setup our survival time
        survivalTimeRemaining = survivalTimeMax;
        // start our countdown
        StartCoroutine(Countdown());
        // set our spawn interval
        spawnRate = survivalTimeMax / (enemies.Count);
        // start spawning enemies
        StartCoroutine(SpawnEnemy());
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
        }
    }

    // spawn enemies at the pre-defined interval throughout our spawnpoints
    IEnumerator SpawnEnemy()
    {
        // debug
        Debug.Log("Spawn rate: " + spawnRate);
        // wait for the spawn
        yield return new WaitForSeconds(spawnRate);
        // what kind of enemy are we spawning?
        if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.ground)
        {
            // spawn at the ground spawn points
            Instantiate(enemies[(int)spawnIndex], groundSpawnPoints[Random.Range(0, groundSpawnPoints.Count)].position, Quaternion.identity);
        }

        if (enemies[(int)spawnIndex].GetComponent<EnemyClass>().enemyType == EnemyClass.enemyTypes.flying)
        {
            // spawn at the ground spawn points
            Instantiate(enemies[(int)spawnIndex], flyingSpawnPoints[Random.Range(0, flyingSpawnPoints.Count)].position, Quaternion.identity);
        }


        // spawn one at a spawn point
        // restart
        StartCoroutine(SpawnEnemy());
    }

    public override void EndChallenge()
    {
        // when our challenge is over, kill all the enemies
        foreach (GameObject enemy in activeEnemies)
        {
            // kill that enemy
            enemy.GetComponent<EnemyClass>().OnDeath(); // right now all enemies have OnDeath functions, list is gameobjects so we can mod it later
        }
    }
}
