using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment_ShotgunCharge : WeaponAttachment
{
    /// this class is used with a shotgun to charge it's shots and allow them to travel further
    /// in reality it actually just fires a larger shot out of the same origin point, which is charged inside here
    /// 

    float charge, chargeMax; // our charge and charge max in seconds
    bool charging; // are we charging?

    // when our mouse button goes down, start charging
    public override void MouseButtonDown1() => charging = true;

    // when our mouse button goes up, stop charging, reset charge
    public override void MouseButtonUp1() => charging = false;

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
}
