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
    public List<GameObject> weapons = new List<GameObject>();
    public List<Transform> weaponCosmeticStorageSlots = new List<Transform>(); // the list of spaces to keep our weapons
    public WeaponClass currentWeapon; // the current weapon in the player's hands
    [SerializeField] int currentWeaponInt; 
    [SerializeField] Transform weaponContainer;

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

    // swap between weapons
    private void Update()
    {
        // moving up and down with the scroll input
        ProcessScrollInput();
    }

    // what we run to process the input
    void ProcessScrollInput()
    {
        // scrolling up
        if (Input.mouseScrollDelta.y > 0)
        {
            // rewrite with new loop
            
            if (currentWeaponInt <= weapons.Count)
            {
                if (currentWeaponInt + 1 >= weapons.Count)
                {
                    currentWeaponInt = 0; Debug.Log("setting int");
                } else if (currentWeaponInt + 1 < weapons.Count)
                {
                    currentWeaponInt++; Debug.Log("adding int");
                }
            }
            UpdateCurrentWeapon();
        }

        // scrolling down 
        if (Input.mouseScrollDelta.y < 0)
        {
            if (currentWeaponInt >= 0)
            {
                if (currentWeaponInt - 1 < 0)
                {
                    // remember to loop here
                    currentWeaponInt = weapons.Count;
                }

                if (currentWeaponInt - 1 >= 0)
                {
                    currentWeaponInt--;
                }
            }
            UpdateCurrentWeapon();
        }
    }

    void UpdateCurrentWeapon()
    {
        // turn off all the weapons
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // enable the current weapon
        weapons[currentWeaponInt].SetActive(true);
        UpdateCosmeticSlots();
    }

    // update our cosmetics
    void UpdateCosmeticSlots()
    {
        // for each weapon in our inventory, spawn their model on the body of the player
        for (int i = 0; i < weapons.Count; i++)
        {
            Instantiate(weapons[i].GetComponent<WeaponClass>().weaponModel, weaponCosmeticStorageSlots[i]);
            // turn off the one we are holding
            if (i == currentWeaponInt)
            {
                weaponCosmeticStorageSlots[i].gameObject.SetActive(false);
            }
        }
    }

    // run this when we are ready to grab a new weapon
    void GrabWeapon()
    {
        // then turn off the renderer of our active weapon in storage
        weaponCosmeticStorageSlots[currentWeaponInt].GetChild(0).gameObject.SetActive(false);

        // then set our hand targets on our animator to the hand targets on the gun
        PlayerInverseKinematicsController.instance.targetRightHand.localPosition = currentWeapon.rightHandPos.localPosition;
        PlayerInverseKinematicsController.instance.targetLeftHand.localPosition = currentWeapon.leftHandPos.localPosition;
    }


}
