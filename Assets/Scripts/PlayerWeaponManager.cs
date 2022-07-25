using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    /// <summary>
    /// Script exists to manage the weapons that the player currently has
    /// </summary>
    /// 

    // the three weapon slots that the player has
    public WeaponClass primaryWeapon, secondaryWeapon, tertiaryWeapon;
    public WeaponClass currentWeapon; // the current weapon in the player's hands

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

}
