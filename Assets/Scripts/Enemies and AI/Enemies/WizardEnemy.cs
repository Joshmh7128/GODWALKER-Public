using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : MonoBehaviour
{
    // the wizard flies to a point nearby the player and fires a fireball that picks up speed
    [SerializeField] Transform playerTransform; // our player's transform
    [SerializeField] Transform headTransform; // our head's transform
    [SerializeField] float HP, maxHP; // our HP variables
    [SerializeField] Vector3 newPos; // the place we want to move to
    [SerializeField] float speed; // our movement speed
    [SerializeField] float hangtime; // the amount of time to wait before deciding on a new movement direction
    [SerializeField] float randomRadius; // the amount of time to wait before deciding on a new movement direction
    [SerializeField] float spherecastRadius; // the spherecast radius
    bool runningBehaviour; // are we running our enemy ai?
    private void Start()
    {
        // set our transform if we don't have one
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
    }

    private void Update()
    {
        // have our head look at the player
        headTransform.LookAt(playerTransform, Vector3.up);
        // have our body rotate on the x to look at the player
        transform.LookAt(new Vector3(playerTransform.position.x, 0, playerTransform.position.y), Vector3.up);

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
        bool spotFree = false;
        // decide on a new position based off of the player's position, but not if that position is occupied
        while (!spotFree)
        {
            // from our player's position, move around them on the X and Z
            Vector3 checkPos = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
            // spherecast to that position to see if we can move there
            Vector3 direction = (transform.position - newPos).normalized;
            RaycastHit hitInfo;
            if (!Physics.SphereCast(transform.position, spherecastRadius, direction, out hitInfo))
            {
                // set the free position to our new movement target
                newPos = checkPos;
                spotFree = true;
            }
        }

        // finish running the behaviour
        runningBehaviour = false;
    }
}
