using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_RandomAirMovement : EnemyBehaviour
{
    /// midair movement
    /// 

    Vector3 targetPos; // our target position we are always moving to
    bool active; // are we active?
    [SerializeField] float speed; // how fast do we move?
    [SerializeField] float bodyRadius; // how big is our body?
    [SerializeField] float moveDistance; // how far do we move per check?
    [SerializeField] bool drawGizmos;
    [SerializeField] bool playerRelative; // is this relative to the player?
    [SerializeField] bool divePlayer; // should we dive at the player
    [SerializeField] float divePlayerHangTime; // how long before diving at the player?
    [SerializeField] LookAtPlayer lookAtPlayer;
    [SerializeField] GameObject cosmeticParent; // our cosmetic parent

    public override IEnumerator MainCoroutine()
    {
        // lerp to our target position
        active = true;

        // store the position of the player in targetPos
        if (divePlayer) 
        { 
            cosmeticParent.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(behaviourTime);
    }

    // choose a new position on the x axis
    void ChoosePos()
    {
        if (!playerRelative)
            targetPos = transform.position + new Vector3(Random.Range(-moveDistance, moveDistance), 0, Random.Range(-moveDistance, moveDistance));

        Vector3 pos = new Vector3(PlayerController.instance.transform.position.x, enemyClass.transform.position.y, PlayerController.instance.transform.position.z);

        if (playerRelative)
            targetPos = pos + new Vector3(Random.Range(-moveDistance, moveDistance), 0, Random.Range(-moveDistance, moveDistance));

    }

    public void FixedUpdate()
    {
        // move if active
        if (active && !divePlayer)
        {
            // check if we are going to run into anything
            Vector3 dir = targetPos - enemyClass.transform.position; 
            Ray ray = new Ray(enemyClass.transform.position, dir);

            // if we hit anything, stop moving
            if (Physics.SphereCast(ray, bodyRadius*1.5f, bodyRadius * 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                ChoosePos();
            }

            // other than that, move towards our target position
            enemyClass.transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.fixedDeltaTime);

            // when we reach our position, choose a new one
            if (Vector3.Distance(enemyClass.transform.position, targetPos) < bodyRadius)
            {
                ChoosePos();
            }

        }

        // if we dive bomb the player, lock on the position of the player, then dive bomb them
        if (divePlayer)
        {
            // move forward
            if (active)
                lookAtPlayer.transform.position += (lookAtPlayer.transform.forward * speed * Time.deltaTime);

            if (divePlayerHangTime >= 0)
            {
                divePlayerHangTime -= Time.deltaTime;
            }

            if (divePlayerHangTime < 0)
            {
                lookAtPlayer.enabled = false;
                targetPos = PlayerController.instance.transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerStatManager.instance.TakeDamage(10f);
            Destroy(enemyClass.gameObject);
        }

        if (other.transform.tag != "Enemy" && other.transform.tag != "Player")
        Destroy(enemyClass.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPos, 5);
        }
    }

}
