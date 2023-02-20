using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAbility_SlagFlamethrower : PlayerCombatAbility
{
    // this is our flamethrower that shoots slag 
    [SerializeField] GameObject flamthrowerObject; // our flamethrower projectile object

    bool canUse;

    // our use main
    public override void UseMain()
    {
        if (charge == chargeMax)
        {
            canUse = false;
        }
    }

    // run our fixedupdate
    public void FixedUpdate()
    {
        // if we cant use our ability that means it is in use, so fire
        if (!canUse && charge > 0)
        {
            Instantiate(flamthrowerObject, transform.position, Quaternion.identity, null);
            charge -= useRate * Time.fixedDeltaTime;
        }
    }

    // timer
    IEnumerator Timer()
    {
        // burn our charge
        yield return new WaitForSeconds(charge);
        // then reset our charge
        charge = 0;

    }

}
