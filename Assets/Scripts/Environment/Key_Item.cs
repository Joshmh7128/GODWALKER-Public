using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key_Item : ItemClass
{
    // instance
    PlayerStatManager playerStatManager;
    [SerializeField] ItemUIHandler uiHandler;
    [SerializeField] GameObject vfx;

    private void Start()
    {
        // set instance
        playerStatManager = PlayerStatManager.instance;
    }

    public void Update()
    {
        ProcessPickup();
    }

    // if we can grab this item
    void ProcessPickup()
    {
        // if we can grab and can pickup
        if (canGrab & Input.GetKeyDown(KeyCode.E))
        {
            playerStatManager.keyAmount++;
            // spawn fx on player
            Instantiate(vfx, PlayerController.instance.transform.position + Vector3.up * 0.5f, vfx.transform.rotation);
            // then destroy object
            Destroy(uiHandler.gameObject);
            Destroy(gameObject);
        }
    }
}
