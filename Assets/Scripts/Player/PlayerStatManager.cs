using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

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
    public float health, maxHealth; // the player's health
    [SerializeField] float damageCooldown, damageCooldownMax; // how long we are unable to take damage for after taking damage

    // our UI variables
    [SerializeField] CanvasGroup hurtUIGroup; // flash this when we take damage
    [SerializeField] VolumeProfile normalProfile, hurt1Profile, hurt2Profile, hurt3Profile; // activate each of these at different health % levels to change the post processing as we get more and more hurt
    [SerializeField] Volume mainVolume; // our main volume
    [SerializeField] Slider healthSlider, healthLerpSlider; // our health slider and our lerp slider
    [SerializeField] Text hpReadout; // set this to be our current / max hp

    bool hasDied; // have we died?

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
            KillPlayer();
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
            // trigger an on jump effect
            PlayerBodyPartManager.instance.CallParts("OnPlayerTakeDamage");
        }

        // even if we don't take damage trigger the UI to react as if we are taking damage
        HurtUIFlash(); // flash our UI
        HurtPlayerIK(); // make our player freak out on hit
        // update our post
        ChoosePostProcessing();
    }

    // all our UI processing overtime
    void ProcessUI()
    {
        // reset our hurtflash
        if (hurtUIGroup.alpha > 0)
            hurtUIGroup.alpha -= 0.3f * Time.deltaTime;

        // sync up our health bars
        healthSlider.value = health / maxHealth;
        healthLerpSlider.value = Mathf.Lerp(healthLerpSlider.value, healthSlider.value, 2*Time.deltaTime);

        // check the % of HP we are at
        hpReadout.text = "Health: " + health + " / " + maxHealth;
    }

    void HurtUIFlash()
    {
        hurtUIGroup.alpha = 1;
    }

    // run the player ik
    void HurtPlayerIK()
    {
        // change intensity as needed
        PlayerInverseKinematicsController.instance.ApplyHurtKickRecoil(-0.4f, -10f, -10f);
    }

    // choose which post processing to show
    void ChoosePostProcessing()
    {
        float currentHP = health / maxHealth;

        // Debug.Log(currentHP);
        // 75% or more = normal volume
        if (currentHP > 0.75f)
        { mainVolume.profile = normalProfile; }

        // 75% to 50% = hurt 1
        if (currentHP < 0.75f && currentHP > 0.50f)
        { mainVolume.profile = hurt1Profile; }       
        
        // 50% to 33% = hurt 2
        if (currentHP < 0.50f && currentHP > 0.33f)
        { mainVolume.profile = hurt2Profile; }
        
        // 33% to 0% = hurt 3
        if (currentHP < 0.33f)
        { mainVolume.profile = hurt3Profile; }
    }

    // kill our player
    void KillPlayer()
    {
        // if we havent died yet
        if (!hasDied)
        {
            // we have now died
            hasDied = true;
            PlayerController.instance.OnPlayerDeath();
            PlayerCameraController.instance.OnPlayerDeath();
            // JUST FOR THE PROTOTYPE stop all enemy behaviours in the test arena
            Debug.LogWarning("Enemy Behaviours are not being stopped in all Arenas");
            FindObjectOfType<ArenaHandler>().StopAllEnemyBehaviours();
        }
    }
}
