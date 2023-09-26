using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedWeaponElement : MonoBehaviour
{
    // this is a part of a weapon which animates when we fire
    public enum AnimationType
    {
        pingpongSoft, pingpongHard, grow, growtate, spin
    }

    [SerializeField] AnimationType animType;

    [Header("Universal Speed Variables")]
    [SerializeField] float speed;
    [SerializeField] float speedReturnDelta;
    float speedCurrent; // the speed of our animation
    [Header("Pingpong Position Variables")]
    [SerializeField] Vector3 rest;
    [SerializeField] Vector3 pingpongPos; // our resting position, and the position of our pingpong to go to
    [Header("Rotation Variables")]
    [SerializeField] Vector3 restRotation;
    [SerializeField] Vector3 growtateRot;
    [SerializeField] Vector3 rotationAxes;
    bool wantRest = true; // do we want to be at our resting position right now?

    // run this when we want to animate
    public void RunAnimation()
    {
        Debug.Log("animation playing");
        switch (animType)
        {
            case AnimationType.pingpongSoft:
                speedCurrent = speed; // set our speed!
                wantRest = false; // we do not want to be resting, play the animation!
                break;

            case AnimationType.pingpongHard:
                speedCurrent = speed; // set our speed!
                wantRest = false; // we do not want to be resting, play the animation!
                break;

            case AnimationType.grow:
                break;
            
            case AnimationType.growtate:
                break;
            
            case AnimationType.spin:
                speedCurrent = speed; // set our speed to our current speed to start the spin
                break;
        }
    }

    // process our speed every update
    void ProcessSpeed()
    {
        // if our speed is not at 0, move it back towards 0 at our delta
        if (Mathf.Abs(speedCurrent) != 0)
            speedCurrent = Mathf.MoveTowards(speedCurrent, 0, speedReturnDelta * Time.deltaTime);
    }

    void ProcessAnimation()
    {
        // move our animation in local space depending on which type it is
        if (animType == AnimationType.pingpongSoft)
        {
            if (wantRest)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, rest, speedCurrent * Time.deltaTime);
            }

            if (!wantRest)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, pingpongPos, speedCurrent * Time.deltaTime);

                // when we reach our destination, go back
                if (Vector3.Distance(transform.localPosition, pingpongPos) < 0.01f)
                {
                    wantRest = true;
                }
            }
        }
        
        // move our animation in local space depending on which type it is
        if (animType == AnimationType.pingpongHard)
        {
            if (wantRest)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, rest, speedCurrent * Time.deltaTime);
            }

            if (!wantRest)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pingpongPos, speedCurrent * Time.deltaTime);

                // when we reach our destination, go back
                if (Vector3.Distance(transform.localPosition, pingpongPos) < 0.01f)
                {
                    wantRest = true;
                }
            }
        }
   
        // are we spinning our object?
        if (animType == AnimationType.spin)
        {
            // rotation done by current speed 
            transform.localRotation *= Quaternion.Euler(rotationAxes * speedCurrent * Time.deltaTime);
        }
    }

    // runs every render tick
    private void Update()
    {
        // process our speed and make sure it is running correctly
        ProcessSpeed();
        // process our animation
        ProcessAnimation();
    }
}
