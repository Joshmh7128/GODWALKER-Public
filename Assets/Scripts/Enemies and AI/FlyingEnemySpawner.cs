using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FlyingEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float activationDistance;
    [SerializeField] Transform player;
    [SerializeField] bool spawnCRisRunning;
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip spawnAnim;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Transform>();
        }
    }

    // fixed update runs 60 times per second
    private void FixedUpdate()
    {
        // check to see how far away the player is
        if (Vector3.Distance(transform.position, player.position) < activationDistance)
        {
            if (!spawnCRisRunning)
            {
                StartCoroutine("SpawnEnemy");
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        spawnCRisRunning = true;
        animator.Play("Spawn");
        yield return new WaitForSeconds(spawnAnim.length);
        Instantiate(enemy, spawnPoint.position, Quaternion.identity, null);
        spawnCRisRunning = false;
    }
}
