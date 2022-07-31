using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIHandler : MonoBehaviour
{
    // this script serves as a handler for dynamic weapon UI. it uses weapon class data to display UI information

    public GameObject weapon; // the weapon we're attached to

    WeaponTypes weaponType; // what weapon is this script attached to?
    enum WeaponTypes
    {
        none, pistol, rifle
    }

    // start runs at the beginning of the game
    private void Start()
    {
        // set the weapon mode based on the scripts attached to us
        SetMode();
    }

    // runs every game tick
    private void Update()
    {
        // run the according UI function each frame
        ProcessUI();
    }

    // check the weapon gameobject to see what kind of UI we're going to be using, then set the state
    void SetMode()
    {
        // if we are using a pistol, go into pistol state
        if (weapon.GetComponent<WeaponClass_Pistol>() != null)
        { weaponType = WeaponTypes.pistol; }

        // if we are using a rifle, go into rifle state
        if (weapon.GetComponent<WeaponClass_Rifle>() != null)
        { weaponType = WeaponTypes.rifle; }
    }

    void ProcessUI()
    {
        // run the according function
        if (weaponType == WeaponTypes.none)
        {
            ProcessUI_None();
        }

        if (weaponType == WeaponTypes.pistol)
        {
            ProcessUI_Pistol();
        }

        if (weaponType == WeaponTypes.rifle)
        {
            ProcessUI_Rifle();
        }
    }

    void ProcessUI_None()
    {
        // nothing here yet
    }

    void ProcessUI_Pistol()
    {
        // display a slider which represented our pistol's ammo amount

    }

    void ProcessUI_Rifle()
    {

    }
}
