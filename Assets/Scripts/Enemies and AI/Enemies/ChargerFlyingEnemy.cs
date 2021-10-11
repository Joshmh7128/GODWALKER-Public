using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement
    [SerializeField] float hangTime; // our chargers hang time
    [SerializeField] float HP; // our HP
    [SerializeField] float maxHP; // our max HP
    [SerializeField] float activationDistance;
    bool canLookAtPlayer; // can we look at the player?
    bool canLinePlayer; // can we look at the player?
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
    [SerializeField] LineRenderer ourLine;
    [SerializeField] Transform lineStart;
    bool lineLocked;
    RaycastHit hit;

    // knockback variables
    Vector3 originForce;
    float knockDistance;

    private void Start()
    {
        ourLine.startColor = new Color(255, 0, 0, 0);
        ourLine.endColor = new Color(255, 0, 0, 0);

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
        // make our indicators red
        foreach (Renderer renderer in indicatorRenderers)
        {
            renderer.material = indicatorRed;
        }
        runningBehaviour = true;
        if (canLinePlayer == false) 
        { 
            runningBehaviour = false;
            yield break;
        }
        currentSpeed = 0;
        // show our line, indicating we will charge at the player, while also facing them
        ourLine.positionCount = 2;
        // can we shoot a line to the player?
        if (canLinePlayer == false)
        {
            runningBehaviour = false;
            yield break;
        }
        lineLocked = false;
        canLookAtPlayer = true;
        animator.Play("Charge Up");
        yield return new WaitForSeconds(hangTime);
        ourLine.material = indicatorRed;
        canLookAtPlayer = false;
        lineLocked = true;

        // can we see the player?
        if (canLinePlayer == false)
        {
            runningBehaviour = false;
            // if we can't see the player, break
            yield break;
        }
        // set our line positions to show we area about to charge the player
        ourLine.SetPosition(1, player.transform.position);
        // display our line
        ourLine.startColor = new Color(255, 0, 0, 255);
        ourLine.endColor = new Color(255, 0, 0, 255);
        // charge at the single position of where we saw the player
        currentSpeed = speed;
        animator.Play("Idle"); 
        newPos = new Vector3(player.transform.position.x + Random.Range(-3,3), player.transform.position.y+1.25f, player.transform.position.z+Random.Range(-3, 3));
        // hang in space for a few moments
        yield return new WaitForSeconds(hangTime);
        ourLine.startColor = new Color(255, 0, 0, 0);
        ourLine.endColor = new Color(255, 0, 0, 0);
        currentSpeed = 0;
        canLookAtPlayer = true;
        yield return new WaitForSeconds(hangTime);

        // restart
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
        // make sure we update our line position so that we are always properly showing it from our body to the player
        ourLine.SetPosition(0, lineStart.position);
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, currentSpeed * Time.deltaTime);
        // calculate knockback
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (new Vector3(originForce.x, originForce.y/4, originForce.z)), knockDistance * Time.deltaTime);

        if (canLookAtPlayer)
        {
            // look at the player
            transform.LookAt(player, Vector3.up);
        }
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
            // Debug.Log("Player is within range");
            if (Physics.Linecast(raycastOrigin.position, player.position, out hit))
            {
                if (hit.transform.tag == ("Player"))
                {
                    canLinePlayer = true;

                    if (!runningBehaviour)
                        StartCoroutine("FlyingBehaviour");
                }
                else
                {
                    canLinePlayer = false;
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
