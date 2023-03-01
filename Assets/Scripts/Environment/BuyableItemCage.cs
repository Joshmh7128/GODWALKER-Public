using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableItemCage : MonoBehaviour
{
    [SerializeField] float cost, interactionDistance; // what is the cost of opening this cage?

    public void Update()
    {
        // if the player presses E and they have more than or equal to the cost of this cage, delete the cage
        if (Input.GetKeyDown(KeyCode.E) && PlayerCurrencyManager.instance.playerCurrencyAmount >= cost 
            && Vector3.Distance(PlayerController.instance.transform.position, transform.position) < interactionDistance)
        {
            // deduct cost
            PlayerCurrencyManager.instance.SubCurrency((int)cost);
            // kill this object
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
