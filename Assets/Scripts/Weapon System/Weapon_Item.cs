using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Item : ItemClass
{
    [SerializeField] bool canGrab; // can the player grab this?

    public GameObject weapon; // the weapon that we'll be using as a source for the swap
    [SerializeField] Transform weaponTransform; // where the weapon will sit

    [SerializeField] GameObject infoPanel; // our info panel to show/hide

    // on start setup our weapon position
    private void Start()
    {
        Instantiate(weapon.GetComponent<WeaponClass>().weaponModel, weaponTransform);
    }

    private void Update()
    {
        if (canGrab && Input.GetKeyDown(KeyCode.E))
        {
            PlayerWeaponManager.instance.PickupWeapon(weapon, this.gameObject);
        }
    }

    public override void ShowInfo()
    {   
        infoPanel.SetActive(true);
    }

    public override void HideInfo()
    {
        infoPanel.SetActive(false);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canGrab = true;
            ShowInfo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canGrab = false;
            HideInfo();
        }
    }
}
