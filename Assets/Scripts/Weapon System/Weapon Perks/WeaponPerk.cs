using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponPerk : MonoBehaviour
{
    // triggers the effect on the weapon perk itself, not the weapon object
    public virtual void TriggerEvent(WeaponPerkManager.Events triggeredEvent) { }

    // add / remove ourselves from the weapon manager when we become active / inactive. temporary effects remain active as their own objects.
    public virtual void OnEnable() => WeaponPerkManager.instance.activeWeaponPerks.Add(this);
    public virtual void OnDisable() => WeaponPerkManager.instance.activeWeaponPerks.Remove(this);
}
