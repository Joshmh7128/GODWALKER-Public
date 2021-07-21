using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnEnemy");
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(15f);
        Instantiate(enemy, spawnPoint.position, Quaternion.identity, null);
        StartCoroutine("SpawnEnemy");
    }
}
