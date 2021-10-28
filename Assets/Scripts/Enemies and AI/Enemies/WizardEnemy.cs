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
    [SerializeField] WizardAnimAssistant wizardAnimAssistant;

    private void Start()
    {
        // make sure our HP is at max
        HP = maxHP;
        HPslider.maxValue = maxHP;


        GameObject.Find("Enemy Manager").GetComponent<EnemyManager>().enemies.Add(gameObject);

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
    }

    private void FixedUpdate()
    {
        // if we can see the player run our behaviour
        RaycastHit hit;
        if (Physics.Linecast(transform.position, playerTransform.position, out hit))
        {
            if (hit.transform.tag == ("Player"))
            {
                if (!runningBehaviour)
                    StartCoroutine(WizardBehaviour());
            }
        }
    }

    IEnumerator WizardBehaviour()
    {
        runningBehaviour = true;
        // wait for our hangtime before deciding on a new direction
        yield return new WaitForSeconds(hangtime);
        // decide on a new position based off of the player's position, but not if that position is occupied
        Vector3 direction = transform.position - playerTransform.position;
        // from our player's position, move around them on the X and Z
        Vector3 checkPos = new Vector3(playerTransform.position.x + Random.Range(-randomRadius, randomRadius), playerTransform.position.y+2f, playerTransform.position.z + Random.Range(-randomRadius, randomRadius));
        // spherecast to that position to see if we can move there
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position, spherecastRadius, direction, out hit, Mathf.Infinity))
        {
            // set the free position to our new movement target
            newPos = checkPos;
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
        // lower HP
        HP -= dmg;
    }
}
