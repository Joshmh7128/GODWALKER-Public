using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_ToughHide : BodyPartClass
{
    // whenever we take damage, start a timer for 10 seconds. current weapon deals 20% more damage in this window
    bool buff; // are we buffing?
    float buffAmount; // this is our buff amount
    bool counting; // are we counting?
    [SerializeField] GameObject FX; // activate when our effect is active

    public override void OnPlayerTakeDamage()
    {
        // if we are not counting, start our counter
        if (!counting)
        {
            StartCoroutine(Counter());
        }
    }

    // call this when the player takes damage
    IEnumerator Counter()
    {
        // we've started counting
        counting = true;
        FX.SetActive(true);
        // apply the damage
        ApplyDamageBuff();
        yield return new WaitForSecondsRealtime(10f); 
        // reset the damage mods of all weapons
        foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
        {
            weapon.GetComponent<WeaponClass>().damageMod = 0;
        }
        // we're done counting, so reset
        FX.SetActive(false);
        counting = false;
    }

    // when we swap weapons, recalculate our buff
    public override void OnWeaponSwap()
    {
        // calculate
        CalculateDamage();
        // then apply it
        ApplyDamageBuff();
    }

    // apply our damage
    void ApplyDamageBuff()
    {
        // only if we are counting apply the damage bonus
        if (counting)
        PlayerWeaponManager.instance.currentWeapon.damageMod = buffAmount;
    }

    // calculate how much damage we are dealing in addition
    void CalculateDamage()
    {
        float damage = PlayerWeaponManager.instance.currentWeapon.damage;
        buffAmount = damage * 0.2f;
    }

}
