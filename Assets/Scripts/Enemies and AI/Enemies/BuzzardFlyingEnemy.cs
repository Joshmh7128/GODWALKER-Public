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
    [SerializeField] float maxHP; // our max HP
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
        // make sure our HP is at max
        HP = maxHP;
        HPslider.maxValue = maxHP;

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

        // set our original newpos to our starting pos
        newPos = transform.position;
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

        // check out height
        // downwards raycast to keep above the ground
        if (Physics.Raycast(transform.position, Vector3.down, 7f))
        {
            tooLow = true;
        }
        else { tooLow = false;  }

        // pick an unnoccupied point in space
        if (tooLow == false)
        {
            // calculate movement variables
            float xMove = Random.Range(-randomRadius, randomRadius);
            float yMove = Random.Range(-randomRadius, randomRadius);
            float zMove = Random.Range(-randomRadius, randomRadius);
            // fire a ray to see if there is anything in the path of our movement
            if (!Physics.Linecast(transform.position, transform.position + new Vector3(xMove, yMove, zMove)))
            {
                // if we aren't too low, move up or down all around
                newPos = transform.position + new Vector3(xMove, yMove, zMove); // where are we flying next?
            }
        }
        else
        {
            // calculate movement variables
            float xMove = Random.Range(-randomRadius, randomRadius);
            float yMove = Random.Range(-randomRadius, randomRadius);
            float zMove = Random.Range(-randomRadius, randomRadius);
            // fire a ray to see if there is anything in the path of our movement
            if (!Physics.Linecast(transform.position, transform.position + new Vector3(xMove, yMove, zMove)))
            {
                // if we are too low move up 
                newPos = transform.position + new Vector3(xMove, Mathf.Abs(yMove), zMove); // where are we flying next?
            }
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
        
    public override void TakeDamage(int dmg)
    {
        if (invincible == false)
        {
            // lower HP
            HP -= dmg;
        }
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
            // make sure to communicate that we have died
            UpgradeSingleton.OnEnemyKill();
            // destroy ourselves
            Destroy(gameObject);
        }
    }

    // fixed update
    private void FixedUpdate()
    {
        // if we are at full health don't show the bar or text
        if (HP == maxHP)
        {
            HPcanvasGroup.alpha = 0;
        }
        else
        {
            HPcanvasGroup.alpha = 1;
        }


        // update our text and bar
        HPTextAmount.text = HP.ToString();
        HPslider.value = HP;

        // is the player nearby us?
        if (Vector3.Distance(transform.position, player.position) < activationDistance)
        {
            if (!runningBehaviour)
            // Debug.Log("Player is within range");
            if (Physics.Linecast(raycastOrigin.position, player.position, out hit))
            {
                if (hit.transform.tag == ("Player"))
                {
                        
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

    }

    // gizmos
    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position, player.position);
    }
}
