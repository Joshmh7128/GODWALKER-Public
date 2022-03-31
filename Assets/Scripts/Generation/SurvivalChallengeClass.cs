using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalChallengeClass : ChallengeClass
{
    /// <summary>
    /// A survival challenge is a challenge where enemies spawn over a period of time.
    /// Once the timer is up, all enemies die and the challenge ends
    /// To facilitate this, on activation begin spawning one enemy at an interval of totalTime/enemyCount
    /// </summary>
    /// 

    [SerializeField] List<GameObject> enemies; // the enemies we will spawn overtime
    [SerializeField] List<Transform> spawnPoints; // all the spawnpoints we can use
    [SerializeField] float survivalTimeMax, survivalTimeRemaining;
    [SerializeField] float spawnRate, spawnIndex;

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

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawnRate);
        Instantiate(enemies[(int)spawnIndex], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
    }

    public override void EndChallenge()
    {

    }
}
