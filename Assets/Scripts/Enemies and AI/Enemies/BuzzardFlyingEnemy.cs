using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzardFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] float shootAnimTime = 0.75f; // out shot animation length
    [SerializeField] float HP; // our HP
    [SerializeField] float activationDistance;
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] GameObject cubePuffDeath; // our death puff
    [SerializeField] GameObject bugPartDrop; // our drop
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;
    [SerializeField] bool tooLow;
    [SerializeField] bool runningBehaviour;
    [SerializeField] Transform enemyManager;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Material indicatorYellow;
    [SerializeField] Material indicatorRed;
    [SerializeField] List<Renderer> indicatorRenderers; // our list of renderers
    RaycastHit hit;

    // knockback variables
    Vector3 originForce;
    float knockDistance;

    private void Start()
    {
        // add to list
        AddToManager();

        // set our parent
        enemyManager = GameObject.Find("Enemy Manager").transform;
        transform.SetParent(enemyManager);

        // find player
        if (player == null)
        {
            player = GameObject.Find("Player").gameObject.transform;
        }
    }

    // make this bug fly around
    IEnumerator FlyingBehaviour()
    {
        runningBehaviour = true;

        // make our indicators red
        foreach (Renderer renderer in indicatorRenderers)
        {
            renderer.material = indicatorRed;
        }

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
        // if the player is close to us, shoot them
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
        // end 
        runningBehaviour = false;
        // make our indicators yellow
        foreach (Renderer renderer in indicatorRenderers)
        {
            renderer.material = indicatorYellow;
        }
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
            // if we're attacking the player drop our item
            if (runningBehaviour)
            { Instantiate(bugPartDrop, transform.position, Quaternion.identity, null); }
            // destroy ourselves
            Destroy(gameObject);
        }
    }

    // fixed update
    private void FixedUpdate()
    {
        // is the player nearby us?
        if (Vector3.Distance(transform.position, player.position) < activationDistance)
        {
            // Debug.Log("Player is within range");
            if (Physics.Linecast(raycastOrigin.position, player.position, out hit))
            {
                if (hit.transform.tag == ("Player"))
                {
                    if (!runningBehaviour)
                    StartCoroutine("FlyingBehaviour");
                }
                else
                {
                    // Debug.Log("No Hit. Tag: " + hit.transform.tag);
                }
            }
        }

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
        else { tooLow = false;  }
    }

    // gizmos
    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position, player.position);
    }
}
