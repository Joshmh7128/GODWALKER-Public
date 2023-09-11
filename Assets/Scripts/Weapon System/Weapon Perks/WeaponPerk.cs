using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponPerk : MonoBehaviour
{
    // triggers the effect on the weapon perk itself, not the weapon object
    public virtual void TriggerEvent(WeaponPerkManager.Events triggeredEvent) { }
}
