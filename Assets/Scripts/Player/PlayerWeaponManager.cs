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
    public GameObject highlightedWeapon; // the weapon which is closest to us

    // setup and set our instance
    public static PlayerWeaponManager instance;
    private void Awake()
    { instance = this; }

    // body parts instance
    PlayerBodyPartManager bodyPartManager;

    // critical hit chance
    public float criticalHitChance; // the chance out of 100 that we will get a critical hit
    public List<float> criticalHitModifiers = new List<float>(); // all the multipliers which go into calculating out critical hit chance

    // weapon ui
    [SerializeField] Transform weaponChargeUIGroup;
    [SerializeField] GameObject weaponChargeUI; // our ui object
     
    private void Start()
    {
        // get our instance
        bodyPartManager = PlayerBodyPartManager.instance;

        // set our weapons list
        foreach (Transform child in weaponContainer)
        { 
            weapons.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        // update
        UpdateCurrentWeapon();

        // then create our weapon charge ui
        UpdateWeaponRechargeUI();
    }

    // swap between weapons
    private void Update()
    {
        // moving up and down with the scroll input
        // ProcessScrollInput();

        // process our weapon pickup cooldown
        ProcessPickupCooldown();

        // calculate our critical hit chance
        CalculateCriticalHitChance();

        // process scroll input
        ProcessScrollInput();

        // process R input
        ProcessKeyInput();

    }

    private void FixedUpdate()
    {
        // process the recharge of our weapons
        ProcessWeaponRecharge();
    }

    float scrollcooldown;

    // what we run to process the input
    void ProcessScrollInput()
    {
        if (scrollcooldown > 0)
        {
            scrollcooldown--;
        }

        // scrolling up
        if (Input.mouseScrollDelta.y < 0 && scrollcooldown <= 0)
        {
            if (currentWeaponInt <= weapons.Count-1)
            {
                if (currentWeaponInt + 1 > weapons.Count-1)
                {
                    currentWeaponInt = 0;
                } else if (currentWeaponInt + 1 < weapons.Count)
                {
                    currentWeaponInt++; 
                }
                
            }
            UpdateCurrentWeapon();

            scrollcooldown = 10;
        }

        // scrolling down 
        if (Input.mouseScrollDelta.y > 0 && scrollcooldown <= 0)
        {
            if (currentWeaponInt > 0)
            {
                currentWeaponInt--;
            } else if (currentWeaponInt <= 0)
            {
                currentWeaponInt = weapons.Count-1;
            }

            UpdateCurrentWeapon();
            scrollcooldown = 10;
        }
    }
       
    // we can also use R to swap between weapons
    void ProcessKeyInput()
    {
        // scrolling up
        if (Input.GetKeyDown(KeyCode.R) && scrollcooldown <= 0)
        {
            if (currentWeaponInt <= weapons.Count - 1)
            {
                if (currentWeaponInt + 1 > weapons.Count - 1)
                {
                    currentWeaponInt = 0;
                }
                else if (currentWeaponInt + 1 < weapons.Count)
                {
                    currentWeaponInt++;
                }

            }
            UpdateCurrentWeapon();

            scrollcooldown = 10;
        }

        // number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponInt = 0;
            UpdateCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponInt = 1;
            UpdateCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeaponInt = 2;
            UpdateCurrentWeapon();
        }
    }

    // what we run to simulate the input
    public void SimulateScrollInput(int direction)
    {
        if (scrollcooldown > 0)
        {
            scrollcooldown--;
        }

        // scrolling up
        if (direction < 0 && scrollcooldown <= 0)
        {
            if (currentWeaponInt <= weapons.Count-1)
            {
                if (currentWeaponInt + 1 > weapons.Count-1)
                {
                    currentWeaponInt = 0; Debug.Log("setting int");
                } else if (currentWeaponInt + 1 < weapons.Count)
                {
                    currentWeaponInt++; Debug.Log("adding int");
                }
                
            }
            UpdateCurrentWeapon();

            scrollcooldown = 10;
        }

        // scrolling down 
        if (direction > 0 && scrollcooldown <= 0)
        {
            if (currentWeaponInt > 0)
            {
                currentWeaponInt--;
            } else if (currentWeaponInt <= 0)
            {
                currentWeaponInt = weapons.Count-1;
            }

            UpdateCurrentWeapon();
            scrollcooldown = 10;
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
        currentWeapon.OnBecomeCurrentWeapon(); // I am become weapon, destroyer of gods
        StartCoroutine(SwitchWeapon());

        // then find and initiate the weapon quick info to make it run a reset
        WeaponQuickInfoHandler.instance.InvokeInfo();

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
        // update current weapon
        UpdateCurrentWeapon();
        // update recharge ui
        UpdateWeaponRechargeUI();
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

        // wait before setting out hands
        yield return new WaitForSecondsRealtime(0.1f);

        // then set our hand targets on our animator to the hand targets on the gun
        PlayerInverseKinematicsController.instance.targetRightHand.localPosition = currentWeapon.rightHandPos.localPosition;
        PlayerInverseKinematicsController.instance.targetLeftHand.localPosition = currentWeapon.leftHandPos.localPosition;
        yield return null;
    }

    // calculate and process our weapon recharge rate
    void ProcessWeaponRecharge()
    {
        // loop through all weapons, if weapon is inactive, add to its recharge variable
        // if recharge variable is more than recharge rate, make recharge 0
        foreach (GameObject weaponObject in weapons)
        {
            // if this is inactive then recharge
            if (weaponObject.activeInHierarchy == false)
            {
                // get weaponclass
                WeaponClass weaponClass = weaponObject.GetComponent<WeaponClass>();
                // then start generating
                if (weaponClass.recharge < weaponClass.rechargeMax)
                    weaponClass.recharge += (weaponClass.rechargeRate * PlayerRageManager.instance.ammoRechargeMods[(int)PlayerRageManager.instance.rageLevel]) * Time.fixedDeltaTime;

                if (weaponClass.recharge > weaponClass.rechargeMax)
                {
                    // check to make sure we're refilling the magazine properly
                    if (weaponClass.currentMagazine < weaponClass.maxMagazine) { weaponClass.currentMagazine += 1; }
                    // then reset recharge
                    weaponClass.recharge = 0;
                }
            }

            // if this weapon is active set its recharge to 0
            if (weaponObject.activeInHierarchy == true)
            {
                // get weaponclass
                WeaponClass weaponClass = weaponObject.GetComponent<WeaponClass>();
                // then stop regen
                weaponClass.recharge = 0;
            }
        }
    }

    // create our weapon recharge UI
    void UpdateWeaponRechargeUI()
    {
        foreach (Transform child in weaponChargeUIGroup)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject weapon in weapons)
        {
            GameObject wo = Instantiate(weaponChargeUI, weaponChargeUIGroup);
            wo.GetComponent<WeaponChargeUIHandler>().ourWeapon = weapon.GetComponent<WeaponClass>();
        }
    }

    // calculate critical hit chance
    void CalculateCriticalHitChance()
    {
        // temporarily set to 100
        float tempCrit = 10;
        // work through the modifiers
        if (criticalHitModifiers.Count > 0)
        {
            foreach (float modifier in criticalHitModifiers)
            {
                tempCrit += modifier;
            }
        }
        else if (criticalHitModifiers.Count <= 0)
        {
            tempCrit = 10f; // set to 10% chance by default
        }

        // then set it
        criticalHitChance = tempCrit;
    }

    // reduce our weapon rage multiplier. this is to be used whenever we advance into a new room
    public void ReduceRageMultiplier()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<WeaponClass>().rageMultiplier -= 0.15f;
            // the player has carried this weapon forward. they used it
            if (PlayerRunStatTracker.instance.weaponUsage.ContainsKey(weapon.GetComponent<WeaponClass>().weaponName))
            {
                PlayerRunStatTracker.instance.weaponUsage[weapon.GetComponent<WeaponClass>().weaponName] = PlayerRunStatTracker.instance.weaponUsage[weapon.GetComponent<WeaponClass>().weaponName] + 1;
            }

            if (!PlayerRunStatTracker.instance.weaponUsage.ContainsKey(weapon.GetComponent<WeaponClass>().weaponName))
            {
                PlayerRunStatTracker.instance.weaponUsage.Add(weapon.GetComponent<WeaponClass>().weaponName, 1);
            } 
        }
    }
}
