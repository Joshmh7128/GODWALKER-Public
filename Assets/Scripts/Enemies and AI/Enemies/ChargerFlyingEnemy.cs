using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerFlyingEnemy : EnemyClass
{
    Vector3 newPos;
    [SerializeField] float speed; // the speed we want to move at
    [SerializeField] float currentSpeed; // the speed we are moving right now
    [SerializeField] float randomRadius; // determines how far he flies per movement 
    [SerializeField] AnimationClip chargeUp; // our charge up time
    [SerializeField] float HP; // our HP
    [SerializeField] float maxHP; // our max HP
    [SerializeField] float activationDistance;
    bool canSeePlayer; // can we look at the player?
    bool canLookAtPlayer; // can we look at the player?
    [SerializeField] GameObject enemyBullet; // the thing we are firing
    [SerializeField] GameObject cubePuffDeath; // our death puff
    [SerializeField] GameObject bugPartDrop; // our drop
    [SerializeField] Transform player;
    [SerializeField] Transform shotOrigin; // where are out shots coming from?
    [SerializeField] Animator animator;
    [SerializeField] Animator hurtAnimator;
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
        canSeePlayer = true;

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

        // make sure we can look at the player from the start to find them
        canSeePlayer = true;
        canLookAtPlayer = true;
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
        
        currentSpeed = 0;
        // show our line, indicating we will charge at the player, while also facing them
        ourLine.positionCount = 2;

        lineLocked = false;
        canSeePlayer = true;
        // set our line positions to show we area about to charge the player
        ourLine.SetPosition(1, player.transform.position);
        Vector3 targetpos;
        if (!canSeePlayer)
        {
            canLookAtPlayer = false;
        }
        // display our line
        ourLine.material = indicatorRed;
        ourLine.startColor = new Color(255, 0, 0, 255);
        ourLine.endColor = new Color(255, 0, 0, 255);
        if (!canSeePlayer)
        {
            canLookAtPlayer = false;
        }
        animator.Play("Charge Up");
        yield return new WaitForSeconds(chargeUp.length);
        if (!canSeePlayer)
        {
            canLookAtPlayer = false;
        }
        lineLocked = true;
        canLookAtPlayer = false;
        targetpos = new Vector3(player.transform.position.x, player.transform.position.y + 1.25f, player.transform.position.z);
        // charge at the single position of where we saw the player
        currentSpeed = speed;
        animator.Play("Idle"); 
        newPos = targetpos;
        // hang in space for a few moments
        yield return new WaitForSeconds(chargeUp.length/2);
        ourLine.startColor = new Color(255, 0, 0, 0);
        ourLine.endColor = new Color(255, 0, 0, 0);
        currentSpeed = 0;
        canLookAtPlayer = true;
        lineLocked = false;

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
        // make sure we update our line position so that we are always properly showing it from our body to the player
        ourLine.SetPosition(0, lineStart.position);
        // move towards our target
        transform.position = Vector3.MoveTowards(transform.position, newPos, currentSpeed * Time.deltaTime);
        // look at the player
        if (canLookAtPlayer)
        {
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

        // Debug.Log("Player is within range");
        if (Physics.Linecast(raycastOrigin.position, player.position, out hit))
        {
            if (hit.transform.tag == ("Player"))
            {
                canSeePlayer = true;
                if (!runningBehaviour)
                    StartCoroutine("FlyingBehaviour");
            }

            if (hit.transform.tag != ("Player"))
            {
                ourLine.startColor = new Color(255, 0, 0, 0);
                ourLine.endColor = new Color(255, 0, 0, 0);
                canSeePlayer = false;
                currentSpeed = 0;
            }
    
        }

        if (Vector3.Distance(raycastOrigin.position, player.position) > activationDistance)
        {
            ourLine.SetPosition(0, lineStart.position);
            // ourLine.SetPosition(1, lineStart.position);
            ourLine.startColor = new Color(0, 0, 0, 0);
            ourLine.endColor = new Color(0, 0, 0, 0);
        }

        if (canSeePlayer && !lineLocked)
        {
            ourLine.SetPosition(0, lineStart.position);
            ourLine.SetPosition(1, player.transform.position);
        }       
        
        if (canSeePlayer && lineLocked)
        {
            ourLine.SetPosition(0, lineStart.position);
            ourLine.SetPosition(1, newPos);
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

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Charger Collision");

        // if this hits the player
        if (col.CompareTag("Player"))
        {
            player.gameObject.GetComponent<PlayerController>().AddHP(-1);
            Camera.main.GetComponent<CameraScript>().shakeDuration += 0.085f;
            Instantiate(cubePuffDeath, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }

        // if this hits a breakable
        if (col.CompareTag("Breakable"))
        {
            // anything with the Breakable tag will be a chunk and have a BreakableBreak function
            col.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
            Instantiate(cubePuffDeath, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }
}
