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
    [SerializeField] AudioSource weaponEquipSource; // our weapon equip audio source
    // weapon item
    [SerializeField] GameObject weaponItem;


    // weapon pickup related
    public float pickupCooldown, pickupCooldownMax = 10f;


    // nearby weapon list
    public List<GameObject> nearbyWeapons; // the weapons near us
    public GameObject nearestWeapon; // the weapon which is closest to us

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

    private void Start()
    {
        // set our weapons list
        foreach (Transform child in weaponContainer)
        { 
            weapons.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        // make sure we spawn our cosmetic weapons
        SpawnCosmeticWeapons();
        // update
        UpdateCurrentWeapon();
    }

    // swap between weapons
    private void Update()
    {
        // moving up and down with the scroll input
        ProcessScrollInput();

        // process our weapon pickup cooldown
        ProcessPickupCooldown();
    }

    private void FixedUpdate()
    {
        // of the weapons near us, find the closest
        ProcessNearestWeapon();
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

    // when we update our weapon
    void UpdateCurrentWeapon()
    {
        if (currentWeapon)
        { currentWeapon.CancelReload(); }

        // turn off all the weapons
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // enable the current weapon
        weapons[currentWeaponInt].SetActive(true);

        currentWeapon = weapons[currentWeaponInt].GetComponent<WeaponClass>();
        UpdateCosmeticSlots();
        StartCoroutine(SwitchWeapon());
    }

    // update our cosmetics
    void UpdateCosmeticSlots()
    {
        // for each weapon in our inventory, turn their weapon on
        for (int i = 0; i < weapons.Count; i++)
        {
            // turn on all the slots
            weaponCosmeticStorageSlots[i].gameObject.SetActive(true);
        }

        // then turn off the renderer of our active weapon in storage
        weaponCosmeticStorageSlots[currentWeaponInt].gameObject.SetActive(false);
    }

    void SpawnCosmeticWeapons()
    {
        // first clean all the weapons
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weaponCosmeticStorageSlots[i].childCount > 0)
            Destroy(weaponCosmeticStorageSlots[i].GetChild(0).gameObject);
        }

        // for each weapon in our inventory, spawn their model on the body of the player
        for (int i = 0; i < weapons.Count; i++)
        {
            GameObject g = Instantiate(weapons[i].GetComponent<WeaponClass>().weaponModel, weaponCosmeticStorageSlots[i], false);
            g.transform.localPosition = Vector3.zero;
        }
    }

    void ProcessPickupCooldown()
    {
        if (pickupCooldown >= 0)
        {
            pickupCooldown--;
        }
    }

    public void PickupWeapon(GameObject newWeaponObject)
    {
        // set our cooldown
        pickupCooldown = pickupCooldownMax;
        // make sure we tell our current weapon it is being dropped
        currentWeapon.OnDrop();
        currentWeapon.CancelReload();
        // first do the weapon itself
        GameObject kill = weapons[currentWeaponInt];
        // instantiate a copy of the weapon we are currently holding
        Weapon_Item copyItem = Instantiate(weaponItem, PlayerController.instance.animationRigParent.position + PlayerController.instance.animationRigParent.forward, Quaternion.identity, null).GetComponent<Weapon_Item>();
        GameObject copyWeapon = Instantiate(kill, copyItem.transform);
        copyWeapon.SetActive(false);
        copyItem.weapon = copyWeapon;
        copyItem.gameObject.GetComponent<Rigidbody>().velocity = PlayerController.instance.animationRigParent.forward * 8;
        copyItem.gameObject.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        Destroy(kill);
        weapons[currentWeaponInt] = Instantiate(newWeaponObject, weaponContainer, false);
        // update cosmetics
        SpawnCosmeticWeapons();
        // update current weapon
        UpdateCurrentWeapon();
        // play our sound effect
        weaponEquipSource.PlayOneShot(weaponEquipSource.clip);
    }

    // run this when we are ready to switch to another weapon
    IEnumerator SwitchWeapon()
    {

        // then turn off the renderer of our active weapon in storage
        weaponCosmeticStorageSlots[currentWeaponInt].gameObject.SetActive(false);
        // reset the rotation of our recoil parent
        PlayerInverseKinematicsController.instance.recoilParent.rotation = Quaternion.identity;
        // interrupt any reloading animations that are playing
        PlayerInverseKinematicsController.instance.reloading = false;

        // then set our hand targets on our animator to the hand targets on the gun
        PlayerInverseKinematicsController.instance.targetRightHand.localPosition = currentWeapon.rightHandPos.localPosition;
        PlayerInverseKinematicsController.instance.targetLeftHand.localPosition = currentWeapon.leftHandPos.localPosition;
        yield return null;
    }

    // find the closest weapon
    void ProcessNearestWeapon()
    {
        if (nearbyWeapons.Count > 0)
        {
            foreach (GameObject weapon in nearbyWeapons)
            {
                if (nearestWeapon == null)
                {
                    nearestWeapon = weapon;
                }
                else
                {
                    // if the distance to our current weapon is less than the distance of the nearest weapon, then make the closer weapon the nearest weapon
                    if (Vector3.Distance(transform.position, weapon.transform.position) < Vector3.Distance(transform.position, nearestWeapon.transform.position))
                    {
                        nearestWeapon = weapon;
                    }
                }
            }
        }
    }

}
