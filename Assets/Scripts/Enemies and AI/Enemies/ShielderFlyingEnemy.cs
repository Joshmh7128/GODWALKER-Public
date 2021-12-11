using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShielderFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] float HP; // our HP
    [SerializeField] float maxHP; // our max HP
    [SerializeField] float activationDistance; // our activation distance
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] GameObject cubePuffDeath; // our death puff
    [SerializeField] GameObject bugPartDrop; // our drop
    [SerializeField] Transform enemyManager;
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Animator animator;
    [SerializeField] Animator hurtAnimator;
    [SerializeField] bool runningBehaviour;
    [SerializeField] bool tooLow;
    [SerializeField] Material indicatorYellow;
    [SerializeField] Material indicatorRed;
    [SerializeField] List<Renderer> indicatorRenderers; // our list of renderers
    public List<Transform> protectedEnemies; // a list of our protected enemies
    [SerializeField] LineRenderer lineRenderer; // our line renderer
    [SerializeField] Transform lineStart; // the start of our line rendering
    int counter; // our frame counter

    // new movement collision detection variables
    [SerializeField] float xMove, yMove, zMove, bodySizeDiameter;
    Vector3 targetPos, targetPosAlt;
    Vector3 hitPos;
    RaycastHit hit;
    RaycastHit lineHit;

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

        // protect our friends
        foreach (Transform friendTransform in protectedEnemies)
        {
            if (friendTransform != null)
            {
                friendTransform.Find("Shield").gameObject.SetActive(true);
                friendTransform.gameObject.GetComponent<EnemyClass>().invincible = true;
            }
        }
    }

    // make this bug fly around
    IEnumerator FlyingBehaviour()
    {
        runningBehaviour = true;

        // protect our friends
        foreach (Transform friendTransform in protectedEnemies)
        {
            if (friendTransform != null)
            {
                friendTransform.Find("Shield").gameObject.SetActive(true);
                friendTransform.gameObject.GetComponent<EnemyClass>().invincible = true;
            }
        }

        // make our indicators red
        foreach (Renderer renderer in indicatorRenderers)
        {
            renderer.material = indicatorRed;
        }

        yield return new WaitForSeconds(Random.Range(2f, 5f));

        // downwards raycast to keep above the ground
        if (Physics.Raycast(transform.position, Vector3.down, 7f))
        {
            tooLow = true;
        }
        else { tooLow = false; }

        // pick an unnoccupied point in space
        // calculate movement variables
        xMove = Random.Range(-randomRadius, randomRadius);
        yMove = Random.Range(-randomRadius, randomRadius);
        zMove = Random.Range(-randomRadius, randomRadius);
        targetPos = new Vector3(xMove, yMove, zMove) + player.position;
        targetPosAlt = new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius)) + player.position;
        // fire a ray to see if there is anything in the path of our movement !Physics.Linecast(transform.position, player.position + new Vector3(xMove, yMove, zMove))

        // if our spherecast fires and hits nothing...
        if (!Physics.SphereCast(transform.position, bodySizeDiameter / 2, (targetPos - transform.position).normalized, out hit, Mathf.Infinity))
        {
            // run a linecast check to make sure we are not clipping through a wall
            if (!Physics.Raycast(transform.position, (targetPos - transform.position).normalized, Mathf.Infinity))
            {
                // move in the direction
                newPos = targetPos;
            }
        }
        // if there is something in the way, move halfway towards out target so that we still move
        if (Physics.SphereCast(transform.position, bodySizeDiameter / 2, (targetPos - transform.position).normalized, out hit, Mathf.Infinity))
        {
            // run a raycast check
            if (Physics.Raycast(transform.position, (targetPos - transform.position).normalized, out lineHit, Mathf.Infinity, Physics.AllLayers))
            {
                // check to make sure the distance we want to move is longer than our body length so we do not clip into a wall
                if (Vector3.Distance(transform.position, lineHit.point) > bodySizeDiameter / 2)
                {
                    // move in the direction at half the length of contact
                    newPos = Vector3.Lerp(transform.position, lineHit.point, 0.5f);
                    hitPos = hit.point;
                }

                // if we can't move there do this with targetPosAlt
                if (Vector3.Distance(transform.position, lineHit.point) < bodySizeDiameter / 2)
                {
                    // if there is something in the way, move halfway towards out target so that we still move
                    if (Physics.SphereCast(transform.position, bodySizeDiameter / 2, (targetPosAlt - transform.position).normalized, out hit, Mathf.Infinity))
                    {
                        // run a raycast check
                        if (Physics.Raycast(transform.position, (targetPosAlt - transform.position).normalized, out lineHit, Mathf.Infinity, Physics.AllLayers))
                        {
                            // check to make sure the distance we want to move is longer than our body length so we do not clip into a wall
                            if (Vector3.Distance(transform.position, lineHit.point) > bodySizeDiameter / 2)
                            {
                                // move in the direction at half the length of contact
                                newPos = Vector3.Lerp(transform.position, lineHit.point, 0.5f);
                                hitPos = hit.point;
                            }
                        }
                    }
                }
            }
        }

        // fly to that place
        currentSpeed = speed;
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
        if (!invincible)
        // play the hurt animation
        hurtAnimator.Play("Hurt");
        // lower HP
        HP -= dmg;
    }

    public void KnockBack(Vector3 originForceLocal, float knockDistanceLocal)
    {
        originForce = originForceLocal;
        knockDistance = knockDistanceLocal;
    }
    // update
    private void Update()
    {
        // setup our HP and values
        HPslider.maxValue = maxHP;
        HPslider.value = HP;
        HPTextAmount.text = HP.ToString();
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, currentSpeed * Time.deltaTime);
        // calculate knockback
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (new Vector3(originForce.x, originForce.y / 4, originForce.z)), knockDistance * Time.deltaTime);
        // look at the player
        transform.LookAt(player);
        // death
        if (HP <= 0)
        {
            // our friends are no longer protected
            foreach (Transform friendTransform in protectedEnemies)
            {
                if (friendTransform != null)
                {
                    friendTransform.Find("Shield").gameObject.SetActive(false);
                    friendTransform.gameObject.GetComponent<EnemyClass>().invincible = false;
                }
            }
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

        // count up our counter
        counter++;

        if (counter >= 2)
        {
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

            // calculate our line renderer to make a path through all of our enemies
            lineRenderer.positionCount = (protectedEnemies.Count * 2) + 1; // add one for our singular overflow
            // set our zeroth position and final position
            lineRenderer.SetPosition(0, lineStart.position);
            lineRenderer.SetPosition((protectedEnemies.Count * 2), lineStart.position);
            // get the positions of our friends
            int mod = 0; // how many positions forward do we have to go?
            // set positions and ensure that the shield is on if we are connected
            foreach (Transform friendTransform in protectedEnemies)
            {
                // make sure the position exists
                if (protectedEnemies[protectedEnemies.IndexOf(friendTransform)] != null)
                {
                    // take the enemy position and draw a line to it
                    lineRenderer.SetPosition(protectedEnemies.IndexOf(friendTransform) + 1 + mod, friendTransform.position);
                    // then draw a line back from 
                    lineRenderer.SetPosition(protectedEnemies.IndexOf(friendTransform) + 2 + mod, transform.position);
                    // make sure we adjust our mod so that we move forward a correct amount of spaces in the protected enemy array
                    mod++;
                    // make sure to enable the shield
                    friendTransform.Find("Shield").gameObject.SetActive(true);
                    friendTransform.gameObject.GetComponent<EnemyClass>().invincible = true;
                }
                else
                {
                    lineRenderer.SetPosition(protectedEnemies.IndexOf(friendTransform) + 1, lineStart.position);
                }
            }

            counter = 0;
        }
    }

    // gizmos
    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(transform.position, player.position);
    }
}
