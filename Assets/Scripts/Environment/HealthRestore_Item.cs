using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRestore_Item : ItemClass
{
    [SerializeField] float healthRestore; // how much does this item increase max health?
    string infoString; // our info
    [SerializeField] ItemUIHandler uiHandler; // our UI handler
    [SerializeField] GameObject vfx; // our vfx
    [SerializeField] bool autoPickup; // does the player pick this up by walking over it?
    [SerializeField] float autoPickupDistance; // how far from the player is this picked up?

    // our instance
    PlayerStatManager playerStatManager;

    private void Start()
    {
        // get instance
        playerStatManager = PlayerStatManager.instance;

        // set our stats
        SetStats();
    }

    void SetStats()
    {
        if (healthRestore == 0)
        {
            healthRestore = (int)Random.Range(1, 3) * 10;
        }

        infoString = "Restores " + healthRestore + " Health";
        uiHandler.itemInfo.text = infoString;
    }

    public void Update()
    {
        ProcessPickup();
    }

    // if we can grab this item
    void ProcessPickup()
    {
        // if we can grab and can pickup
        if (!autoPickup)
        {
            if (canGrab & Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("adding " + healthRestore);
                playerStatManager.AddHealth(healthRestore);
                // spawn fx on player
                Instantiate(vfx, PlayerController.instance.transform.position + Vector3.up * 0.5f, vfx.transform.rotation);
                // then destroy object
                Destroy(uiHandler.gameObject);
                Destroy(gameObject);
            }
        }

        if (autoPickup)
        {
            if (Vector3.Distance(transform.position, playerStatManager.transform.position) < autoPickupDistance)
            {
                Debug.Log("adding " + healthRestore);
                playerStatManager.AddHealth(healthRestore);
                // spawn fx on player
                Instantiate(vfx, PlayerController.instance.transform.position + Vector3.up * 0.5f, vfx.transform.rotation);
                // then destroy object
                Destroy(uiHandler.gameObject);
                Destroy(gameObject);
            }
        }



    }
}
