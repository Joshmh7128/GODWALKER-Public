using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_ToughHide : BodyPartClass
{
    // whenever we take damage, start a timer for 10 seconds. current weapon deals 20% more damage in this window
    [SerializeField] float buffAmount; // this is our buff amount
    [SerializeField] bool counting; // are we counting?
    [SerializeField] GameObject VFX; // our vfx

    public override void OnPlayerTakeDamage()
    {
        FindVFX(); // find it everytime we take damage

        // if we are not counting, start our counter
        if (counting == false)
        {
            StartCoroutine("Countdown");
        }
    }

    // call this when the player takes damage
    IEnumerator Countdown()
    {
        // we've started counting
        counting = true;
        VFX.SetActive(true);
        // calculate
        CalculateDamage();
        // apply the damage
        ApplyDamageBuff();
        Debug.Log(buffAmount);
        yield return new WaitForSecondsRealtime(10f); 
        // reset the damage mods of all weapons
        foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
        {
            weapon.GetComponent<WeaponClass>().damageMods.Remove(buffAmount);
        }
        // we're done counting, so reset
        counting = false;
        VFX.SetActive(false);
    }

    // when we swap weapons, recalculate our buff
    public override void OnWeaponSwap()
    {
        // calculate
        CalculateDamage();
        // then apply it
        if (counting)
        ApplyDamageBuff();
    }

    // apply our damage
    void ApplyDamageBuff()
    {
        // only if we are counting apply the damage bonus
        PlayerWeaponManager.instance.currentWeapon.damageMods.Add(buffAmount);
    }

    // calculate how much damage we are dealing in addition
    void CalculateDamage()
    {
        float damage = PlayerWeaponManager.instance.currentWeapon.damage;
        buffAmount = damage * 0.2f;
    }

    // find our visual fx
    void FindVFX()
    {
        // our vfx need to be triggered manually, so on pickup find our effects
        if (PlayerBodyPartManager.instance.torsoPartParents[0].GetChild(0).Find("VFX").gameObject != null)
        VFX = PlayerBodyPartManager.instance.torsoPartParents[0].GetChild(0).Find("VFX").gameObject;
    }

}
