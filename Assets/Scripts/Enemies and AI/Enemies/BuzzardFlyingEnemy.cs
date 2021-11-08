using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzardFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] float HP, maxHP; // our HP
    [SerializeField] float activationDistance;
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] GameObject cubePuffDeath; // our death puff
    [SerializeField] GameObject bugPartDrop; // our drop
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip shootAnim;
    [SerializeField] Animator hurtAnimator;
    [SerializeField] bool tooLow;
    [SerializeField] bool runningBehaviour;
    [SerializeField] Transform enemyManager;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Material indicatorYellow;
    [SerializeField] Material indicatorRed;
    [SerializeField] List<Renderer> indicatorRenderers; // our list of renderers
    [SerializeField] float xMove, yMove, zMove, bodySizeRadius;
    Vector3 targetPos;
    Vector3 hitPos;

    RaycastHit hit;
    RaycastHit lineHit;

    // knockback variables
    Vector3 originForce;
    float knockDistance;

    private void Start()
    {
        // set our original newpos to our starting pos
        newPos = transform.position;

        // make sure our HP is at max
        HP = maxHP;
        HPslider.maxValue = maxHP;

        // add to list
        AddToManager();

        // set our parent
        if (enemyManager)
        {
            enemyManager = GameObject.Find("Enemy Manager").transform;
            transform.SetParent(enemyManager);
        }
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
        // pick an unnoccupied point in space
        // calculate movement variables
        xMove = Random.Range(-randomRadius, randomRadius);
        yMove = Random.Range(-randomRadius, randomRadius);
        zMove = Random.Range(-randomRadius, randomRadius);
        targetPos = new Vector3(xMove, yMove, zMove) + player.position;
        // fire a ray to see if there is anything in the path of our movement !Physics.Linecast(transform.position, player.position + new Vector3(xMove, yMove, zMove))

        // if our spherecast fires and hits nothing...
        if (!Physics.SphereCast(transform.position, bodySizeRadius, (targetPos - transform.position).normalized, out hit, Mathf.Infinity))
        {
            // run a linecast check to make sure we are not clipping through a wall
            if (!Physics.Raycast(transform.position, (targetPos - transform.position).normalized, Mathf.Infinity))
            {
                // move in the direction
                newPos = targetPos;
            }
        } // if there is something in the way, move halfway towards out target so that we still move
        
        if (Physics.SphereCast(transform.position, bodySizeRadius, (targetPos - transform.position).normalized, out hit, Mathf.Infinity))
        {
            // run a linecast check to make sure we are not clipping through a wall
            if (Physics.Raycast(transform.position, (targetPos - transform.position).normalized, out lineHit, Mathf.Infinity, Physics.AllLayers))
            {
                // check to make sure the distance we want to move is longer than our body length so we do not clip into a wall
                if (Vector3.Distance(transform.position, lineHit.point) > bodySizeRadius * 2f)
                {
                    // move in the direction at half the length of contact
                    newPos = Vector3.Lerp(transform.position, lineHit.point, 0.5f);
                    hitPos = hit.point;
                }
            }
        }

        // fly to that place
        currentSpeed = speed;
        // if the player is close to us, shoot them
        // animate shot charge up
        animator.Play("Shoot");
        // wait for the animation to finish
        yield return new WaitForSeconds(shootAnim.length);
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
            // play the hurt animation
            hurtAnimator.Play("Hurt");
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
            // remove ourselves from the roomclass list
            // roomClass.enemyClasses.Remove(this);
            // destroy ourselves
            Destroy(gameObject);
        }
    }

    // fixed update
    private void FixedUpdate()
    {
        // raycast downwards to see if we are too close to the ground
        RaycastHit speedCheck;
        // if our destination is below us
        if (targetPos.y < transform.position.y)
        {  // run the raycast to detect another collider
            if (Physics.Raycast(transform.position, Vector3.down, out speedCheck, bodySizeRadius, Physics.AllLayers))
            {
                // stop moving
                currentSpeed = 0;
            }
        }

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
            {
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
        // Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPos, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hitPos, 5);
    }
}
