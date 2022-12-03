using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponChargeUIHandler : MonoBehaviour
{
    // used to access and show charge of weapons
    public int weaponID; // which weapon are we working with? set when created
    public TextMeshProUGUI weaponNameDisplay; // the name of our weapon to display. set when created
    public Slider chargeSlider; // displays charge, set in here
    public WeaponClass ourWeapon; // the weapon we're working with, set on creation
    public Image chargeImage; // the image of our charge that we're going to change the color of
    public Color charging, idle; // the idle and charging colors

    private void FixedUpdate()
    {
        // process charge
        ProcessCharge();
    }

    void ProcessCharge()
    {
        // get the charge of our associated weapon then update the slider
        if (ourWeapon.gameObject.activeInHierarchy == false)
        {
            chargeImage.color = charging;   
            chargeSlider.value = ourWeapon.recharge / ourWeapon.rechargeMax;
        }
        else
        {
            chargeImage.color = idle;
            chargeSlider.value = ourWeapon.recharge / ourWeapon.rechargeMax;
        }
    }

}
