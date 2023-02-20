using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAbility_SlagFlamethrower : PlayerCombatAbility
{
    // this is our flamethrower that shoots slag 
    [SerializeField] GameObject flamthrowerObject; // our flamethrower projectile object

    // our use main
    public override void UseMain()
    {
        Instantiate(flamthrowerObject, transform.position, Quaternion.identity, null);
    }

}
