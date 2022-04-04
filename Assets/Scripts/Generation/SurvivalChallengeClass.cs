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
    [SerializeField] List<Transform> spawnPoints; // all the spawnpoints we can use
    [SerializeField] float survivalTimeMax, survivalTimeRemaining; // define our gameplay objectives
    float spawnRate, spawnIndex; // automatically determined
    [SerializeField] string challengeType, difficultyLevel, reward, fullInfo; // our info strings

    private void Start()
    {
        // set our info text correctly for this challenge
        fullInfo = challengeType + "\n" + difficultyLevel + "\n" + reward;
        // set that to the text on the canvas
        infoText.text = fullInfo;
    }

    public override void Activate()
    {
        // setup our survival time
        survivalTimeRemaining = survivalTimeMax;
        // start our countdown
        StartCoroutine(Countdown());
        // set our spawn interval
        spawnRate = survivalTimeMax / enemies.Count;
    }

    IEnumerator Countdown()
    {
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
        }
    }

    // spawn enemies at the pre-defined interval throughout our spawnpoints
    IEnumerator SpawnEnemy()
    {
        // wait for the spawn
        yield return new WaitForSeconds(spawnRate);
        // spawn one at a spawn point
        Instantiate(enemies[(int)spawnIndex], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
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
