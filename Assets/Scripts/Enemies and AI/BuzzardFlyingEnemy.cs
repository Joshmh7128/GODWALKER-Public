using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzardFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;
    [SerializeField] float shootAnimTime = 0.75f; // out shot animation length
    [SerializeField] float HP; // our HP
    [SerializeField] GameObject cubePuffDeath; // our death puff
    // knockback variables
    Vector3 originForce;
    float knockDistance;

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
        // wait for the animation to finish
        yield return new WaitForSeconds(shootAnimTime);
        // shoot
        GameObject bullet = Instantiate(enemyBullet, shotOrigin.position, Quaternion.identity, null);
        bullet.GetComponent<EnemyBulletScript>().bulletTarget = player;
        // wait a random amount after
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        currentSpeed = 0;
        // repeat
        StartCoroutine("FlyingBehaviour");
    }

    public override void TakeDamage(int dmg, Vector3 dmgOrigin)
    {
        // lower HP
        HP -= dmg;
        // trigger knockback
        KnockBack(dmgOrigin, 30f);
    }

    public void KnockBack(Vector3 originForceLocal, float knockDistanceLocal)
    {
        originForce = originForceLocal;
        knockDistance = knockDistanceLocal;
    }
    // update
    private void Update()
    {
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, currentSpeed * Time.deltaTime);
        // calculate knockback
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (new Vector3(originForce.x, originForce.y/4, originForce.z)), knockDistance * Time.deltaTime);
        // look at the player
        transform.LookAt(player);
        // death
        if (HP <= 0)
        {
            // spawn a death effect
            Instantiate(cubePuffDeath, transform.position, Quaternion.identity, null);
            // destroy ourselves
            Destroy(gameObject);
        }
    }

    // fixed update
    private void FixedUpdate()
    {
        if (knockDistance > 0)
        {
            knockDistance -= 2f;
        }
    }
}
