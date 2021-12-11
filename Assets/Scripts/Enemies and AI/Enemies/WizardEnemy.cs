using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : EnemyClass
{
    // the wizard flies to a point nearby the player and fires a fireball that picks up speed
    [SerializeField] Transform playerTransform; // our player's transform
    [SerializeField] Transform headTransform; // our head's transform
    [SerializeField] Transform rightShotTransform, leftShotTransform; // our head's transform
    [SerializeField] float HP, maxHP; // our HP variables
    [SerializeField] Vector3 newPos; // the place we want to move to
    [SerializeField] float speed; // our movement speed
    [SerializeField] float hangtime; // the amount of time to wait before deciding on a new movement direction
    [SerializeField] float randomRadius; // the amount of time to wait before deciding on a new movement direction
    [SerializeField] float spherecastRadius; // the spherecast radius
    [SerializeField] Animator animator, hurtAnimator; // our animators
    bool runningBehaviour; // are we running our enemy ai?
    [SerializeField] GameObject projectile; // our projectile prefab
    [SerializeField] GameObject cubePuffParticle, bugPartDrop; // our projectile prefab
    [SerializeField] WizardAnimAssistant wizardAnimAssistant;
    int counter; // our off-frame counter

    // new movement collision detection variables
    [SerializeField] float xMove, yMove, zMove, bodySizeDiameter;
    Vector3 targetPos, targetPosAlt;
    Vector3 hitPos;
    RaycastHit hit;
    RaycastHit lineHit;

    private void Start()
    {
        // make sure our HP is at max
        HP = maxHP;
        HPslider.maxValue = maxHP;

        // add to list
        AddToManager();

        // set our transform if we don't have one
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }

        newPos = transform.position;
    }

    private void Update()
    {
        // have our head look at the player
        headTransform.LookAt(playerTransform, Vector3.up);
        // have our body rotate on the x to look at the player
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z), Vector3.up);

        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);

        // death
        if (HP <= 0)
        {
            // spawn a death effect
            Instantiate(cubePuffParticle, transform.position, Quaternion.identity, null);
            // if we're attacking the player drop our item
            if (runningBehaviour)
            { Instantiate(bugPartDrop, transform.position, Quaternion.identity, null); }
            // make sure to communicate that we have died
            UpgradeSingleton.OnEnemyKill();
            // remove ourselves from the roomclass list
            if (roomClass != null)
            {
               // roomClass.enemyClasses.Remove(this);
            }
            // destroy ourselves
            Destroy(gameObject);

        }
    }

    private void FixedUpdate()
    {
        // count up our counter
        counter++;

        // if we can see the player run our behaviour
        RaycastHit hit;
        if (counter >= 2)
        {
            if (Physics.Linecast(transform.position, playerTransform.position, out hit))
            {
                if (hit.transform.tag == ("Player"))
                {
                    if (!runningBehaviour)
                        StartCoroutine(WizardBehaviour());
                }
            }

            counter = 0;
        }

        // if we are at full health don't show the bar or text
        if (HP == maxHP)
        { HPcanvasGroup.alpha = 0; }
        else
        { HPcanvasGroup.alpha = 1; }

        HPTextAmount.text = HP.ToString();
        HPslider.value = HP;
    }

    IEnumerator WizardBehaviour()
    {
        runningBehaviour = true;
        // wait for our hangtime before deciding on a new direction
        yield return new WaitForSeconds(hangtime);
        // pick an unnoccupied point in space
        // calculate movement variables
        xMove = Random.Range(-randomRadius, randomRadius);
        yMove = 3; // we want to be around the same height as the player
        zMove = Random.Range(-randomRadius, randomRadius);
        targetPos = new Vector3(xMove, yMove, zMove) + playerTransform.position;
        targetPosAlt = new Vector3(Random.Range(-randomRadius, randomRadius), 3f, Random.Range(-randomRadius, randomRadius)) + playerTransform.position;
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

        // play our attack animation 
        animator.Play("Attack");
        // wait for the first frame them fire a projectile
        yield return new WaitUntil(() => wizardAnimAssistant.leftFire);
        // instantiate left projectile
        GameObject leftProjectile = Instantiate(projectile, leftShotTransform.position, Quaternion.identity, null);
        leftProjectile.GetComponent<EnemyBulletScript>().bulletTarget = playerTransform;
        // wait the rest of the time
        yield return new WaitUntil(() => wizardAnimAssistant.rightFire);
        // instantiate right projectile
        GameObject rightProjectile = Instantiate(projectile, rightShotTransform.position, Quaternion.identity, null);
        rightProjectile.GetComponent<EnemyBulletScript>().bulletTarget = playerTransform;
        // finish running the behaviour
        runningBehaviour = false;
    }

    public IEnumerator WaitForFrames(int frameCount)
    {
        if (frameCount <= 0)
        {
            
        }

        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }

    public override void TakeDamage(int dmg)
    {
        // play the hurt animation
        hurtAnimator.Play("Hurt");
        // lower HP if not invincible
        if (!invincible)
        {
            HP -= dmg;
        }
    }
}
