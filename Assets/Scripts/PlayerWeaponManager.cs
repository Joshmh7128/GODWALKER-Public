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

    // weapon item prefab
    [SerializeField] GameObject weaponItemPrefab;

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

    private void Start()
    {
        // setup the cosmetic elements of our weapons on the character
        UpdateWeaponCosmetics();
        UpdateCurrentWeapon();
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
            UpdateCurrentWeapon();
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
            UpdateCurrentWeapon();
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

    void UpdateCurrentWeapon()
    {
        // if we are changing down, show the previous weapon we were just holding in its slot

        // swap to the correct list entry
        currentWeapon = weaponClasses[currentWeaponInt];
        // turn on all the renderers
        for (int i = 0; i < weaponStorageSlots.Count; i++)
        {
            if (weaponStorageSlots[i].childCount > 0)
            {
                weaponStorageSlots[i].GetChild(0).gameObject.SetActive(true);
            }
        }

        // then grab the new weapon
        StartCoroutine(GrabWeapon());

    }

    // run this when we are ready to grab a new weapon
    IEnumerator GrabWeapon()
    {
        float elapsedTime = 0;
        float waitTime = 0f; // raise this for crude grabbing animations
        // then set our hand targets on our animator to the hand targets on the gun
        while (elapsedTime < waitTime)
        {
            // lerp to the current slot
            PlayerInverseKinematicsController.instance.targetRightHand.position = Vector3.Lerp(PlayerInverseKinematicsController.instance.targetRightHand.position, weaponStorageSlots[currentWeaponInt].position, 8 * Time.deltaTime);
            PlayerInverseKinematicsController.instance.targetLeftHand.position = Vector3.Lerp(PlayerInverseKinematicsController.instance.targetRightHand.position, weaponStorageSlots[currentWeaponInt].position, 8 * Time.deltaTime);
            // elapsed time add
            elapsedTime += Time.deltaTime;
            yield return null;  
        }

        yield return new WaitForSeconds(0f);
        // then turn off the renderer of our active weapon in storage
        weaponStorageSlots[currentWeaponInt].GetChild(0).gameObject.SetActive(false);

        // then set our hand targets on our animator to the hand targets on the gun
        PlayerInverseKinematicsController.instance.targetRightHand.localPosition = currentWeapon.rightHandPos.localPosition;
        PlayerInverseKinematicsController.instance.targetLeftHand.localPosition = currentWeapon.leftHandPos.localPosition;
        // change the weapon prefab in our origin slot
        Destroy(weaponContainer.GetChild(0).gameObject);
        Instantiate(currentWeapon.gameObject, weaponContainer);

    }

    // pickup weapon
    public void PickupWeapon(GameObject weapon, GameObject breaker)
    {
        // throw our an exact replica of the weapon we are currently holding
        Weapon_Item weapon_item = Instantiate(weaponItemPrefab, transform.position, Quaternion.identity, null).GetComponent<Weapon_Item>();
        weapon_item.weapon = currentWeapon.gameObject;

        // set our current weapon to the weapon that we want to pickup
        currentWeapon = weapon.GetComponent<WeaponClass>();
        weaponClasses[currentWeaponInt] = weapon.GetComponent<WeaponClass>();
        // setup cosmetics
        GameObject destroying = weaponStorageSlots[currentWeaponInt].GetChild(0).gameObject;
        Destroy(destroying);
        Instantiate(weapon.GetComponent<WeaponClass>().weaponModel, weaponStorageSlots[currentWeaponInt]);
        // destroy the other weapon object that is on the ground
        breaker.GetComponent<ItemClass>().DestroyGameObject();
        // update
        UpdateCurrentWeapon();
    }
}
