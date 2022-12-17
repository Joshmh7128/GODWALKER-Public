using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment_ChargeShot : WeaponAttachment
{
    /// this class is used with a shotgun to charge it's shots and allow them to travel further
    /// in reality it actually just fires a larger shot out of the same origin point, which is charged inside here
    /// 

    float charge, chargeMax; // our charge and charge max in seconds
    bool charging; // are we charging?
    [SerializeField] Transform hinge; // our animation hinge
    [SerializeField] AudioSource sfx; // our sound source
    Quaternion hingeTargetRotation; // our target rotation for our hinge

    // when our mouse button goes down, start charging
    public override void MouseButtonDown1()
    {
        charging = true;
        // set target rotation to up
        hingeTargetRotation = Quaternion.Euler(0, 0, 0);
    }

    // when our mouse button goes up, stop charging, reset charge
    public override void MouseButtonUp1() 
    { 
        charging = false;
        // set target rotation to down
        hingeTargetRotation = Quaternion.Euler(0, -180, 0);
    }

    // run our fixed update to update out charge
    private void FixedUpdate()
    {
        ProcessCharge();
    }

    // run our charge
    void ProcessCharge()
    {
        // add to our charge
        if (charging && charge < chargeMax)
            charge += Time.fixedDeltaTime;

        // reset our charge
        if (!charging)
            charge = 0;
    }

    // process the rotation of our charge attachment
    void ProcessRotation()
    {

    }
}
