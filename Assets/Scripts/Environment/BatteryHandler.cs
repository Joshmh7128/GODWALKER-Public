using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHandler : MonoBehaviour
{
    /// this script handles each individual battery
    /// it calculates charge, changes the charge color and scale, and enables and disables the associated elements
    /// 

    float charge, chargeMax; // our current and maximum charge
    [SerializeField] Transform chargeTransform; // the transform we'll be adjusting the scale of depending on our charge amount

    // every frame calculate the charge
    void ProcessCharge()
    {
        // calculate the change in charge
        if (charge >= 0)
        {
            charge -= Time.deltaTime;
        }

        // set the scale of the charge transform on a scale of 0 to 1 equivalent to the current charge out of its max
        float i = charge / chargeMax;
        chargeTransform.localScale = new Vector3(0, i, 0);

    }

    // set our charge to its maximum
    void ChargeBattery()
    {
        charge = chargeMax; 
    }
}
