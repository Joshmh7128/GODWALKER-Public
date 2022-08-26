using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    /// <summary>
    /// this script handles all of the stats for the player, and all of the UI associated with them
    /// </summary>

    // setup our instance
    public static PlayerStatManager instance;
    private void Awake()
    {
        instance = this;
    }

    // our main public variables
    public float health; // the player's health
    [SerializeField] float damageCooldown, damageCooldownMax; // how long we are unable to take damage for after taking damage

    // our UI variables
    [SerializeField] CanvasGroup hurtUIGroup; // flash this when we take damage
    [SerializeField] GameObject hurtVolumes; // activate each of these at different health % levels to change the post processing as we get more and more hurt


    // runs 60 times per second
    private void FixedUpdate()
    {
        // check our HP every frame to make sure we are supposed to be alive
        ProcessCheckHealth();

        // process our UI updates each frame
        ProcessUI();
    }


    // check our health every frame
    void ProcessCheckHealth()
    {
        // run our damage cooldown
        if (damageCooldown > 0)
        {
            damageCooldown--;
        }

        // run our damage check
        if (health <= 0)
        {
            // die
        }
    }

    // take damage
    public void TakeDamage(float damageAmount)
    {
        // don't take damage if we are un able to
        if (damageCooldown <= 0)
        {
            // set our damage cooldown
            damageCooldown = damageCooldownMax;
            // set our health
            health -= damageAmount;
        }

        // even if we don't take damage trigger the UI to react as if we are taking damage
        HurtUIFlash();

    }

    void ProcessUI()
    {
        // reset our hurtflash
        if (hurtUIGroup.alpha > 0)
            hurtUIGroup.alpha--;
    }

    void HurtUIFlash()
    {
        hurtUIGroup.alpha = 1;
    }

    // 
    void KillPlayer()
    {

    }
}
