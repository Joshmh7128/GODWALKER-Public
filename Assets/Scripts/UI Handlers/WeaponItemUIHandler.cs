using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItemUIHandler : MonoBehaviour
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

    [SerializeField] Weapon_Item weapon_Item; // our weapon item
    WeaponClass weapon_Class; // our weapon class to pull information from

    [SerializeField] CanvasGroup weapon_CanvasGroup;

    float closeWait; // our wait to close time

    string weaponInfo;
    [SerializeField] Text weaponInfoText, weaponNameText;

    private void Start()
    {
        // set the weaponclass
        weapon_Class = weapon_Item.weapon.GetComponent<WeaponClass>();
        // set our information
        SetInfo();
    }

    private void FixedUpdate()
    {
        if (closeWait > 0)
        { 
            weapon_CanvasGroup.alpha += Time.deltaTime*10;
            closeWait -= Time.deltaTime; 
        }

        if (closeWait <= 0)
        {
            weapon_CanvasGroup.alpha -= Time.deltaTime;

            if (weapon_CanvasGroup.alpha <= 0)
            {
                showPanel = false;
            }
        }

        // check for death
        if (weapon_Item == null)
        {
            Destroy(gameObject);
        }

    }

    // set the info panel of our weapon
    void SetInfo()
    {
        int accuracy = (int) (100 - ((weapon_Class.spreadXDelta + weapon_Class.spreadYDelta) * 100));
        int firerate = (int) (((60 - weapon_Class.firerate)/6) * 10) - 60; // our fire rate is in frames per second, so we want to divide it by 60 to show how many bullets per second we fire

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
}
