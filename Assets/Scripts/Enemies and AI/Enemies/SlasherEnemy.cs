using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherEnemy : EnemyClass
{
    [SerializeField] float HP, maxHP; // our current and maximum hp
    [SerializeField] float speed; // our max and current speeds
    [SerializeField] Animator mainAnimator, hurtAnimator; // our animators
    [SerializeField] List<int> attackPattern; // our attack pattern
    [SerializeField] List<AnimationClip> animationClips; // our animation clips
    [SerializeField] List<string> animationClipStrings; // our animation clips
    [SerializeField] LineRenderer targetLineRenderer; // our target line renderers
    [SerializeField] bool isActive, trackPlayer; // are we active yet?
    [SerializeField] enum targetStatePositions { none, center, player};
    [SerializeField] targetStatePositions targetStatePosition; // what is our taget position?
    [SerializeField] Vector3 targetPosition, targetStatePositionCenter, dampenedPlayerPosition; // our target position of the player
    [SerializeField] Transform lineStartPosition; // our line start position
    [SerializeField] GameObject player;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // get our player if we can 
        player = GameObject.Find("Player");
        // start our attacks, only in testing do we start this in the start. We will trigger this in the bossfight via the boss chunk
        StartCoroutine(AttackCoroutine(attackPattern));
    }

    IEnumerator AttackCoroutine(List<int> attackPatterns)
    {
        // do our attacks
        foreach (int i in attackPatterns)
        {
            // play the attack
            mainAnimator.Play(animationClipStrings[i]);
            // wait for the attack to finish
            yield return new WaitForSeconds(animationClips[i].length);
        }
    }

    private void Update()
    {
        // dampen our player position
        dampenedPlayerPosition = Vector3.SmoothDamp(dampenedPlayerPosition, player.transform.position, ref velocity, 0.3f);

        // update our tracking of the player
        if (trackPlayer)
        {
            // if we are tracking the player, update our player position and the end of our line
            targetLineRenderer.SetPosition(0, lineStartPosition.position);
            targetLineRenderer.SetPosition(1, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            // look at the player
            transform.LookAt(dampenedPlayerPosition);
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

    }

    // how we take damage
    public override void TakeDamage(int dmg)
    {
        // play the hurt animation
        hurtAnimator.Play("Hurt");
        // lower HP
        HP -= dmg;
    }

    // triggers dealing damage
    private void OnTriggerEnter(Collider col)
    {
        // if this hits the player
        if (col.CompareTag("Player"))
        {
            player.gameObject.GetComponent<PlayerController>().AddHP(-1);
            Camera.main.GetComponent<CameraScript>().shakeDuration += 0.085f;
        }

        // if this hits a breakable
        if (col.CompareTag("Breakable"))
        {
            // anything with the Breakable tag will be a chunk and have a BreakableBreak function
            col.gameObject.GetComponent<BreakableChunk>().BreakableBreak();
        }
    }
}
