using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalkFoot : MonoBehaviour
{
    // vars
    [SerializeField] Transform targetFoot;
    [SerializeField] Vector3 currentPos; // where we are now
    [SerializeField] Vector3 sitPos; // where feet can be
    [SerializeField] float maxDelta; // the maximum distance we can be from a foot
    [SerializeField] float maxSpread; // randomization maximum
    [SerializeField] float legSpeed; // how fast do legs move
    public bool onSit; // are we standing on the foot position?

    // Start
    private void Start()
    {
        // so we have a current position at the beginning
        currentPos = targetFoot.position;
    }

    bool newPos = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void MoveFoot()
    {
        float maxDeltaTemp = Random.Range(maxDelta, maxDelta * 1.5f);

        if (Vector3.Distance(currentPos, targetFoot.position) > maxDeltaTemp)
        {
            // declare a new sitPos
            sitPos = currentPos + (targetFoot.position - currentPos) * 1.8f;

            if (newPos == false)
            {
                newPos = true;
            }
            onSit = false;
        }

        if (onSit == false)
        {
            currentPos = Vector3.MoveTowards(currentPos, sitPos, legSpeed * Time.deltaTime);
            transform.position = currentPos;

            if (transform.position == sitPos)
            {
                onSit = true;
            }
        }
    }
}