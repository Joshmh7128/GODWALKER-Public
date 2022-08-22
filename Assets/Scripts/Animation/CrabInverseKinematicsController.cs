using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabInverseKinematicsController : MonoBehaviour
{
    /// <summary>
    /// This script serves to move each of the four feet on crab enemies
    /// It also controls their hurt reactions with IK
    /// </summary>

    // our static positions, IK controllers, foot target positions, transforms
    [SerializeField] List<Transform> staticFeetPositions, feetIKControllers, targetFeetPositions;
    [SerializeField] List<bool> canMove; // our list of bools whic tells the feet if they are allowed to move
    [SerializeField] float forwardStepDelta = 1f; // how far forward we step
    [SerializeField] float footSpeed = 1f; // how fast our feet move
    [SerializeField] float footStepEpsilon; // how far away our foot needs to be from our static position in order to step
    [SerializeField] float footSwapTime; // how long does each foot have to stay put

    // this script supports two humanoid animators, up to four legs and four arms
    [SerializeField] LocalIKHandler frontAnimator, backAnimator;

    // start
    private void Start()
    {
        // setting up our feet
        SetupTargetFeet();
        // start our canmove control
        //StartCoroutine(CanMoveControl());
    }

    private void SetupTargetFeet()
    {
        // unparent each target foot transform
        foreach (Transform foot in targetFeetPositions)
        {
            foot.transform.parent = null;
        }

        // unparent our controllers as well
        foreach (Transform foot in feetIKControllers)
        {
            foot.transform.parent = null;
        }
    }

    IEnumerator CanMoveControl()
    {
        // then flip each bool
        for (int i = 0; i < canMove.Count; i++)
        {
            canMove[i] = true;
            yield return new WaitForSeconds(footSwapTime);
            canMove[i] = false;
        }
        //StartCoroutine(CanMoveControl());
    }

    private void FixedUpdate()
    {
        // process our target positions
        ProcessLegTargetPositions();
        // move our legs
        ProcessLegIKControllers();
    }

    // for calculating out leg target position
    void ProcessLegTargetPositions()
    {
        // for each static leg position...
        for (int i = 0; i < staticFeetPositions.Count; i++)
        {
            Vector3 dir = staticFeetPositions[i].position - targetFeetPositions[i].position;
            if (Vector3.Distance(targetFeetPositions[i].position, staticFeetPositions[i].position) > footStepEpsilon)
            {
                // update our target leg positions to be at the static leg position + a little bit forward based 
                // get our direction towards our static leg pos from our target leg pos
                targetFeetPositions[i].position = staticFeetPositions[i].position + (dir.normalized * forwardStepDelta);

                // then mod our footstep epsilon
            }

        }
    }

    // lerping our feet, call recursively in an update
    void ProcessLegIKControllers()
    {
        // for each controller, move towards their target
        for (int i = 0; i < feetIKControllers.Count; i++)
        {
            // check if we can move
            if (canMove[i])
            feetIKControllers[i].position = Vector3.MoveTowards(feetIKControllers[i].position, targetFeetPositions[i].position, footSpeed * Time.deltaTime);
        }


    }
}
