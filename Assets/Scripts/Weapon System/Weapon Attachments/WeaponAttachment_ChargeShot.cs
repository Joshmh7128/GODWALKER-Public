using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponAttachment_ChargeShot : WeaponAttachment
{
    /// this class is used with a shotgun to charge it's shots and allow them to travel further
    /// in reality it actually just fires a larger shot out of the same origin point, which is charged inside here
    /// 

    [SerializeField] float charge, chargeMax; // our charge and charge max in seconds
    bool charging; // are we charging?
    [SerializeField] Transform hinge; // our animation hinge
    [SerializeField] AudioSource sfx; // our sound source
    Quaternion hingeTargetRotation; // our target rotation for our hinge
    [SerializeField] float hingeStart; // start x rot of hinge
    [SerializeField] Slider chargeSlider;
    [SerializeField] TextMeshProUGUI chargeText;

    // when our mouse button goes down, start charging
    public override void MouseButtonDown1()
    {
        charging = true;
        // set target rotation to up
        hingeTargetRotation = Quaternion.Euler(0, 0, 0);
        // play sound
        sfx.pitch = 1.5f;
        sfx.Play();
    }

    // when our mouse button goes up, stop charging, reset charge
    public override void MouseButtonUp1() 
    { 
        charging = false;
        // set target rotation to down
        hingeTargetRotation = Quaternion.Euler(hingeStart, 0, 0);      
        // play sound
        sfx.pitch = 1f;
        sfx.Play();
    }

    // if we press mouse 0, fire the enemy class's projectile with damage and distance multiplied by our charge
    public override void MouseButtonDown0()
    {
        weaponClass.FireCustom(charge * 3, weaponClass.damage * charge * 3);
    }

    // run our fixed update to update out charge
    private void FixedUpdate()
    {
        ProcessCharge();
        ProcessRotation();
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

        chargeSlider.value = charge / chargeMax;
        chargeText.text = ((int)((charge / chargeMax) * 100)).ToString();
    }

    // process the rotation of our charge attachment
    void ProcessRotation()
    {
        hinge.localRotation = Quaternion.RotateTowards(hinge.localRotation, hingeTargetRotation, 30f);
    }
}
