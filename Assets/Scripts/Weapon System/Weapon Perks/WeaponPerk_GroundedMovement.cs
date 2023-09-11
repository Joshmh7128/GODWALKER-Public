using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPerk_GroundedMovement : WeaponPerk
{
    // this is an active effect which incentivizes moving while grounded
    // if the player is moving while grounded, increase their damage and rage generation
    [SerializeField] float damageMultiplier, rageMultiplier;
    bool active; // is this effect active?

    public override void TriggerEvent(WeaponPerkManager.Events triggeredEvent)
    {
        // if we are moving and grounded
        if (triggeredEvent == WeaponPerkManager.Events.moveGrounded)
        {
            // make sure our weapon manager doesnt have the 
            if (!active)
            {
                ActivateEffect();
                return;
            }
        } 
        // if we are not moving and grounded
        else if (triggeredEvent == WeaponPerkManager.Events.stillGrounded || triggeredEvent == WeaponPerkManager.Events.becomeUngrounded)
        {
            if (active)
                DeactivateEffect();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DeactivateEffect();
    }

    // activate the damage multiplier
    void ActivateEffect()
    {
        active = true;
        PlayerWeaponManager.instance.damageModifiers.Add(damageMultiplier);
        PlayerWeaponManager.instance.rageModifiers.Add(rageMultiplier);
        // Debug.Log("player moving and receiving boost");
    }

    // deactivate the damage multiplier
    void DeactivateEffect()
    {
        active = false; 
        PlayerWeaponManager.instance.damageModifiers.Remove(damageMultiplier);
        PlayerWeaponManager.instance.rageModifiers.Remove(rageMultiplier);
        // Debug.Log("player not moving");
    }
}
