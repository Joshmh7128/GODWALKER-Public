using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIHandler : MonoBehaviour
{
    // should we show our panel?
    public bool showPanel
    {
        set
        {
            panel.gameObject.SetActive(value);
        }
    } 

    [SerializeField] Transform panel; // our panel transform parent

    public Vector3 hitPoint // where is the camera script hitting on this object?
    {
        set { panel.transform.position = value; panel.transform.LookAt(Camera.main.transform.position); closeWait = 0.5f; }
    }

    // our enum for item types
    public enum ItemTypes
    {
        Weapon, BodyPart
    }

    public ItemTypes itemType; // what type of item are we?
    
    [Header("- Weapon Data -")]
    // information for weapons
    public Weapon_Item weapon_Item; // our weapon item
    public WeaponClass weapon_Class; // our weapon class to pull information from
    string weaponInfo;
    [SerializeField] Text weaponInfoText, weaponNameText;


    // information for bodyparts
    public BodyPartClass body_Part;
    [Header("- Body Part Data -")]
    public BodyPart_Item bodyPart_Item;
    public Text bodyPartName, currentRightName, currentLeftName;
    public Text bodyPartInfo, currentRightInfo, currentLeftInfo;
    BodyPartClass rightClass, leftClass; // for local ease

    [Header("- Canvas Groups -")]
    [SerializeField] CanvasGroup info_CanvasGroup;
    [SerializeField] GameObject additional_info;
    float closeWait; // our wait to close time

    private void Start()
    {
        GetInfo();
        // set our information
        SetInfo();
    }

    void GetInfo()
    {
        // for weapons
        if (itemType == ItemTypes.Weapon)
        {
            // set the weaponclass
            if (weapon_Item)
                weapon_Class = weapon_Item.weapon.GetComponent<WeaponClass>();
        }

        // for bodyparts
        if (itemType == ItemTypes.BodyPart)
        {
            // get the part class manually
            body_Part = bodyPart_Item.bodyPartObject.GetComponent<BodyPartClass>();

            // check the bodypart type and whether or not we use additional panels
            if (body_Part.bodyPartType == BodyPartClass.BodyPartTypes.Arm || body_Part.bodyPartType == BodyPartClass.BodyPartTypes.Leg)
            {
                additional_info.SetActive(true);
            }
        }
    }

    private void FixedUpdate()
    {
        // processing opening / closing our panel
        if (closeWait > 0)
        { 
            info_CanvasGroup.alpha += Time.deltaTime*10;

            closeWait -= Time.deltaTime; 
        }

        if (closeWait <= 0)
        {
            info_CanvasGroup.alpha -= Time.deltaTime;

            if (info_CanvasGroup.alpha <= 0)
            {
                showPanel = false;
            }
        }

        // check for death
        if (weapon_Item == null && body_Part == null)
        {
            // Destroy(gameObject);
        }
    }

    // set the info panel of our weapon
    void SetInfo()
    {
        // for our weapons
        if (itemType == ItemTypes.Weapon)
        {
            int accuracy = (int)(90 - ((weapon_Class.spreadXDelta + weapon_Class.spreadYDelta) * 100));
            int firerate = (int)(((60 - weapon_Class.firerate) / 6) * 10) - 60; // our fire rate is in frames per second, so we want to divide it by 60 to show how many bullets per second we fire

            // set the info for our player
            weaponInfo =
                 weapon_Class.damage + "\n" +
                 accuracy + "\n" +
                 firerate + "\n" +
                 weapon_Class.reloadTime + "\n" +
                 weapon_Class.maxMagazine + "\n" +
                 weapon_Class.weaponElement.ToString();

            weaponInfoText.text = weaponInfo;
            weaponNameText.text = weapon_Class.weaponName;
        }

        // for our bodyparts
        if (itemType == ItemTypes.BodyPart)
        {
            #region // setup our current classes based on type 
            if (body_Part.bodyPartType == BodyPartClass.BodyPartTypes.Arm)
            {
                rightClass = PlayerBodyPartManager.instance.rightArmPartClass;
                leftClass = PlayerBodyPartManager.instance.leftArmPartClass;
            }

            if (body_Part.bodyPartType == BodyPartClass.BodyPartTypes.Leg)
            {
                rightClass = PlayerBodyPartManager.instance.rightLegPartClass;
                leftClass = PlayerBodyPartManager.instance.leftLegPartClass;
            }
            #endregion

            // set our information
            bodyPartName.text = body_Part.bodyPartName;
            bodyPartInfo.text = body_Part.descriptiveInfo;
            // right arm
            currentRightName.text = rightClass.bodyPartName;
            currentRightInfo.text = rightClass.descriptiveInfo;
            currentLeftName.text = leftClass.bodyPartName;
            currentLeftInfo.text = leftClass.descriptiveInfo;
        }
    }
}
