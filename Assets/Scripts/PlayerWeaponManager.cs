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
    public List<WeaponClass> weaponClasses = new List<WeaponClass>(); // the list of current weapons that we have
    public List<Transform> weaponStorageSlots = new List<Transform>(); // the list of spaces to keep our weapons
    public WeaponClass currentWeapon; // the current weapon in the player's hands
    [SerializeField] int currentWeaponInt, maxWeapons; 
    [SerializeField] Transform weaponContainer;

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

    private void Start()
    {
        // setup the cosmetic elements of our weapons on the character
        UpdateWeaponCosmetics();
    }

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
            if (currentWeaponInt <= weaponClasses.Count)
            {
                if (currentWeaponInt + 1 >= weaponClasses.Count)
                {
                    currentWeaponInt = 0; Debug.Log("setting int");
                } else if (currentWeaponInt + 1 < weaponClasses.Count)
                {
                    currentWeaponInt++; Debug.Log("adding int");
                }
            }
            UpdateCurrentWeapon((int)Input.mouseScrollDelta.y);
        }

        // scrolling down 
        if (Input.mouseScrollDelta.y < 0)
        {
            if (currentWeaponInt >= 0)
            {
                if (currentWeaponInt - 1 < 0)
                {
                    currentWeaponInt = weaponClasses.Count;
                }

                if (currentWeaponInt - 1 >= 0)
                {
                    currentWeaponInt--;
                }
            }
            UpdateCurrentWeapon((int)Input.mouseScrollDelta.y);
        }
    }

    void UpdateWeaponCosmetics()
    {
        // clean our all the weaponstorage slots
        foreach (Transform slot in weaponStorageSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0));
            }
        }

        // then instantiate each weapon the player is currently holding on to their body
        foreach (WeaponClass weaponClass in weaponClasses)
        {
            Instantiate(weaponClass.weaponModel, weaponStorageSlots[weaponClasses.IndexOf(weaponClass)]);
        }

        // then turn off the renderer of our active weapon in storage
        weaponStorageSlots[currentWeaponInt].GetChild(0).gameObject.SetActive(false);
    }

    void UpdateCurrentWeapon(int changeDirection)
    {
        // if we are changing down, show the previous weapon we were just holding in its slot

        // swap to the correct list entry
        currentWeapon = weaponClasses[currentWeaponInt];
        // change the weapon prefab in our origin slot
        Destroy(weaponContainer.GetChild(0).gameObject);
        Instantiate(currentWeapon.gameObject, weaponContainer);
        // turn on all the renderers
        for (int i = 0; i < weaponStorageSlots.Count; i++)
        {
            if (weaponStorageSlots[i].childCount > 0)
            {
                weaponContainer.GetChild(0).gameObject.SetActive(true);
            }
        }

        // then turn off the renderer of our active weapon in storage
        weaponStorageSlots[currentWeaponInt].GetChild(0).gameObject.SetActive(false);
    }
}
