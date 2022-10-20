using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler : MonoBehaviour
{ 
    // script is made to handle the player's teleportation from it to it's partner

    PlayerController playerController;

    [SerializeField] TeleporterHandler partner;
    public float cooldown, cooldownMax = 5; // cooldown in seconds
    
    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (cooldown <= 0)
            {
                TriggerTeleport();
            }
        }
    }

    // when the player collides with this teleporter, teleport the player to our partner, and then set both of our cooldowns
    private void TriggerTeleport()
    {
        playerController.Teleport(partner.transform.position + Vector3.up);
        cooldown = cooldownMax;
        partner.cooldown = cooldownMax;
    }


}
