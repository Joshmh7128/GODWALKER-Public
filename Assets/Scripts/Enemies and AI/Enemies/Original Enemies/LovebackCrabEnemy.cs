using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LovebackCrabEnemy : MonoBehaviour
{
    // this script has the loveback crab walk around aimlessly in a little radius
    [SerializeField] float randomRadius; // how far can we walk around?
    [SerializeField] float waitTime; // how long should we wait before finding a new place to walk
    [SerializeField] float speed; // our speed
    float currentSpeed; // our currnt Speed
    Vector3 newPos; // the new position we are moving to
    Vector3 startPos; // our starting position
    Vector3 targetPos; // our target pos that we are lerping to every frame
    float interpolationCount = 400; // the amount of frames to lerp within
    float elapsedFrames = 0; // the amount of frames we have elapsed
    float interpolationRatio;

    private void Start()
    {
        StartCoroutine(WalkingCycle()); // start the walking cycle
        startPos = transform.position;
    }

    IEnumerator WalkingCycle()
    {
        elapsedFrames = 0; // reset our lerp frame variable
        currentSpeed = 0;
        newPos = startPos + new Vector3(Random.Range(-randomRadius, randomRadius), 0, Random.Range(-randomRadius, randomRadius)); // find a new target position
        yield return new WaitForSeconds(waitTime);
        currentSpeed = speed; // start walking
        yield return new WaitForSeconds(Random.Range(2,5)); // let the crab walk for a random amount of time towards their destination
        StartCoroutine(WalkingCycle()); // restart the walking cycle
    }

    private void FixedUpdate()
    {
        interpolationRatio = elapsedFrames / interpolationCount;
        targetPos = Vector3.Lerp(targetPos, newPos, interpolationRatio);
        if (elapsedFrames < interpolationCount)
        { elapsedFrames++; }

        if (elapsedFrames > interpolationCount)
        { elapsedFrames = 0; }

        // move to our new destination
        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
        // look at our new destination
        transform.LookAt(targetPos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(startPos, randomRadius);
    }
}
