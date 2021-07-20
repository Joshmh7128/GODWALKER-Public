using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzardFlyingEnemy : MonoBehaviour
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;

    private void Start()
    {
        StartCoroutine("FlyingBehaviour");
    }

    // make this bug fly around
    IEnumerator FlyingBehaviour()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        // pick a point in space
        newPos = transform.position + new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius/4, randomRadius/4), Random.Range(-randomRadius, randomRadius)); // where are we flying next?
        // fly to that place
        currentSpeed = speed;
        // animate shot charge up
        animator.Play("Shoot");
        // shoot
        GameObject bullet = Instantiate(enemyBullet, shotOrigin.position, Quaternion.identity, null);
        bullet.GetComponent<EnemyBulletScript>().bulletTarget = player;
        // wait
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        currentSpeed = 0;
        // repeat
        StartCoroutine("FlyingBehaviour");
    }

    // update
    private void Update()
    {
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, currentSpeed * Time.deltaTime);
        // look at the player
        transform.LookAt(player);
    }
}
