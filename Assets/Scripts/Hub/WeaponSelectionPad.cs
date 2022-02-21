using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionPad : MonoBehaviour
{
    [SerializeField] PlayerController.weaponTypes targetType;
    [SerializeField] string panelMessage;

    // if we come in to contact with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().currentWeapon = targetType;
        }
    }
}
