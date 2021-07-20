using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] float shootAnimTime = 1f; // out shot animation length
    [SerializeField] float HP; // our HP
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] GameObject cubePuffDeath; // our death puff
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;
    [SerializeField] bool tooLow;

    // knockback variables
    Vector3 originForce;
    float knockDistance;

    private void Start()
    {
        StartCoroutine("FlyingBehaviour");
        if (player == null)
        {
            player = GameObject.Find("Player").gameObject.transform;
        }
    }

    // make this bug fly around
    IEnumerator FlyingBehaviour()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        // pick a point in space
        if (tooLow == false)
        {   // if we aren't too low move down
            newPos = transform.position + new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius / 4, -randomRadius / 2), Random.Range(-randomRadius, randomRadius)); // where are we flying next?
        }
        else
        {   // if we are too low move up 
            newPos = transform.position + new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(randomRadius / 4, randomRadius / 2), Random.Range(-randomRadius, randomRadius)); // where are we flying next?
        }

        // fly to that place
        currentSpeed = speed;
        // animation is designed to shoot first then fall back
        GameObject bullet = Instantiate(enemyBullet, shotOrigin.position, Quaternion.identity, null);
        bullet.GetComponent<EnemySphereBomb>().playerController = player.gameObject.GetComponent<PlayerController>();
        animator.Play("DropperShoot");
        // wait for the animation to finish
        yield return new WaitForSeconds(shootAnimTime);
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
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (new Vector3(originForce.x, originForce.y / 4, originForce.z)), knockDistance * Time.deltaTime);
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
        // knockback reduction
        if (knockDistance > 0)
        {
            knockDistance -= 2f;
        }

        // downwards raycast to keep above the ground
        if (Physics.Raycast(transform.position, Vector3.down, 7f))
        {
            tooLow = true;
        }
        else { tooLow = false; }
    }
}
