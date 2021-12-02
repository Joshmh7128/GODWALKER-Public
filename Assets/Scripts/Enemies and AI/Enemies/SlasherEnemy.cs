using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherEnemy : EnemyClass
{
    [SerializeField] float HP, maxHP; // our current and maximum hp
    [SerializeField] float speed, shielderSpawnRadius; // our max and current speeds
    [SerializeField] Animator mainAnimator, hurtAnimator; // our animators
    [SerializeField] List<int> attackPattern; // our attack pattern
    [SerializeField] List<AnimationClip> animationClips; // our animation clips
    [SerializeField] List<string> animationClipStrings; // our animation clips
    [SerializeField] LineRenderer targetLineRenderer; // our target line renderers
    [SerializeField] bool isActive, trackPlayer; // are we active yet?
    int canHurtCounter;
    [SerializeField] enum targetStatePositions { none, center, player};
    [SerializeField] targetStatePositions targetStatePosition; // what is our taget position?
    [SerializeField] Vector3 targetPosition, targetStatePositionCenter, dampenedPlayerPosition; // our target position of the player
    [SerializeField] Transform lineStartPosition, projectileLaunchPosition; // our line start position
    [SerializeField] GameObject player;
    [SerializeField] GameObject projectile, shielder, shield; // our projectile that we fire, the shielder enemy to support us
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // get our player if we can 
        player = GameObject.Find("Player");
        // start our attacks, only in testing do we start this in the start. We will trigger this in the bossfight via the boss chunk
        StartCoroutine(AttackCoroutine(attackPattern));
        // set our target center position
        targetStatePositionCenter = transform.position;
    }

    int coroutineCount;
    IEnumerator AttackCoroutine(List<int> attackPatterns)
    {
        // do our attacks
        foreach (int i in attackPatterns)
        {
            // play the attack
            mainAnimator.Play(animationClipStrings[i]);
            // wait for the attack to finish
            yield return new WaitForSeconds(animationClips[i].length);
            // count
            coroutineCount++;
        }

        // reset the count and start over
        if (coroutineCount >= attackPatterns.Count)
        {
            coroutineCount = 0;
            StartCoroutine(AttackCoroutine(attackPattern));
        }
    }

    private void Update()
    {

        // update our tracking of the player
        if (trackPlayer)
        {
            // if we are tracking the player, update our player position and the end of our line
            targetLineRenderer.SetPosition(0, lineStartPosition.position);
            targetLineRenderer.SetPosition(1, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            // look at the player
            transform.LookAt(dampenedPlayerPosition);
            // dampen our player position
            dampenedPlayerPosition = Vector3.SmoothDamp(dampenedPlayerPosition, player.transform.position, ref velocity, 0.3f);
        }

        // move to our target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // make sure we look at the player
        // transform.LookAt(player.transform);

        // adjust our target position
        if (targetStatePosition == targetStatePositions.center)
        {
            targetPosition = targetStatePositionCenter;
        }

        if (targetStatePosition == targetStatePositions.player)
        {
            targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }

        // death
        if (HP <= 0)
        {
            // make sure to communicate that we have died
            UpgradeSingleton.OnEnemyKill();
            // remove ourselves from the roomclass list
            // roomClass.enemyClasses.Remove(this);
            // destroy ourselves
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (canHurtCounter > 0)
        {
            canHurtCounter--;
        }

        // setup our HP and values
        HPslider.maxValue = maxHP;
        HPslider.value = HP;
        HPTextAmount.text = HP.ToString();
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

    }

    public void FireProjectile()
    {
        GameObject ourShot = Instantiate(projectile, projectileLaunchPosition.position, Quaternion.identity, null);
        ourShot.GetComponent<EnemyBulletScript>().bulletTarget = player.transform;
    }

    public void CreateShielder()
    {
        if (!shield.activeInHierarchy)
        {
            GameObject ourShielderA = Instantiate(shielder, new Vector3(transform.position.x + 15, transform.position.y + 30, transform.position.z), Quaternion.identity, null);
            ourShielderA.GetComponent<ShielderFlyingEnemy>().protectedEnemies.Add(gameObject.transform);
            GameObject ourShielderB = Instantiate(shielder, new Vector3(transform.position.x - 15, transform.position.y + 30, transform.position.z), Quaternion.identity, null);
            ourShielderB.GetComponent<ShielderFlyingEnemy>().protectedEnemies.Add(gameObject.transform);
            GameObject ourShielderC = Instantiate(shielder, new Vector3(transform.position.x, transform.position.y + 30, transform.position.z + 15), Quaternion.identity, null);
            ourShielderC.GetComponent<ShielderFlyingEnemy>().protectedEnemies.Add(gameObject.transform);
        }
    }

    // how we take damage
    public override void TakeDamage(int dmg)
    {
        // lower HP
        if (!invincible)
        HP -= dmg;
        // play the hurt animation
        hurtAnimator.Play("Hurt");
    }

    // triggers dealing damage
    private void OnTriggerEnter(Collider col)
    {
        // if this hits the player
        if (col.CompareTag("Player"))
        {
            if (canHurtCounter <= 0)
            {
                player.gameObject.GetComponent<PlayerController>().AddHP(-3);
                Camera.main.GetComponent<CameraScript>().shakeDuration += 0.085f;
                canHurtCounter += 10;
            }
        }

        // if this hits a breakable
        if (col.CompareTag("Breakable"))
        {
            // anything with the Breakable tag will be a chunk and have a BreakableBreak function
            col.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
            col.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
            col.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
        }
    }
}
