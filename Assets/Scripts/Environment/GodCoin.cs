using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodCoin : MonoBehaviour
{
    // this coin flies out, hangs, then quickly moves towards the player

    [SerializeField] float hangTime;
    [SerializeField] float moveDelta;
    [SerializeField] float pickupDistance;
    [SerializeField] float speed, finalSpeed; // our movement speed

    [SerializeField] GameObject pickupSound; // our pickup sound

    Transform target; // our player transform

    // start runs when we start
    private void Start()
    {
        // get player transform
        target = PlayerController.instance.transform;

        transform.position += new Vector3(transform.position.x + Random.Range(-1,1), transform.position.y + Random.Range(-1, 1), transform.position.z + Random.Range(-1, 1));    

        // randomize our values a bit
        int b = (Random.Range(0, 2) * 2) - 1;
        hangTime = hangTime * 0.25f * b;

        int c = (Random.Range(0, 2) * 2) - 1;
        moveDelta = moveDelta * 0.25f * c;

        // hangtime
        StartCoroutine(HangTime());
    }

    // our waittime coroutine
    public IEnumerator HangTime()
    {
        finalSpeed = 0;
        yield return new WaitForSeconds(hangTime);
        finalSpeed = speed;
    }


    // calculate our movement
    void ProcessMovement()
    {
        // perform the movement update
        transform.Translate((target.position - transform.position) * finalSpeed * Time.fixedDeltaTime);
        // add to our speed
        finalSpeed += moveDelta;
    }

    void ProcessDistance()
    {
        if (Vector3.Distance(transform.position, target.position) < pickupDistance)
        {
            PlayerCurrencyManager.instance.AddCurrency(1);
            Instantiate(pickupSound, null); // instantiate in world space
            Destroy(gameObject);
        }
    }

    // fixed update to run our movement processes
    private void FixedUpdate()
    {
        // process movement
        ProcessMovement();
        // can we be picked up?
        ProcessDistance();
    }



}
