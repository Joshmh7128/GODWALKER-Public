using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPerkManager : MonoBehaviour
{
    // the list of our active weapon perks, added from weapons and challenges.
    // includes active weapon effects, temporary, and permanent traits
    public List<WeaponPerk> activeWeaponPerks = new List<WeaponPerk>();

    // all of the different events we can call. things like jumping, shooting, taking damage, etc.
    public enum Events
    {
        none, becomeGrounded, moveGrounded
    }

    // this is called externally from other scripts when events are triggered
    // it triggers the events on the weapon perk classes that are active in the list
    // it does not trigger things on the weapon objects themselves
    public void TriggerEvent(Events triggeredEvent)
    {
        foreach (WeaponPerk weaponPerk in activeWeaponPerks)
            weaponPerk.TriggerEvent(triggeredEvent);
    }
}
