using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAbility_SlagFlamethrower : PlayerCombatAbility
{
    // this is our flamethrower that shoots slag 
    [SerializeField] GameObject flamthrowerObject; // our flamethrower projectile object

    bool canUse = true, canCharge = false, beingUsed = false;

    // our use main
    public override void UseMain()
    {
        Debug.Log("using main");

        if (canUse)
        {
            beingUsed = true;
        }
    }

    // run our fixedupdate
    public void FixedUpdate()
    {
        // if we cant use our ability that means it is in use, so fire
        if (beingUsed && charge > 0)
        {
            Instantiate(flamthrowerObject, PlayerController.instance.transform.position, PlayerCameraController.instance.transform.rotation, null);
            charge -= useRate * Time.fixedDeltaTime;
        }

        // when can we charge?
        if (charge <= 0)
        { 
            canUse = false;
            canCharge = true;
            beingUsed = false;
        }

        if (charge >= chargeMax)
        { 
            canCharge = false;
            canUse = true;
        }

        if (canCharge)
        {
            charge += rechargeRateDelta * Time.fixedDeltaTime;
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
