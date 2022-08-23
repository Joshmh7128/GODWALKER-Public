using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Crab : EnemyClass
{
    [SerializeField] LocalIKHandler hurtHandler;

    public override void GetHurt()
    {
        // when we get hurt, kick
        hurtHandler.KickLookPos(30f);
        // flash
        StartCoroutine(HurtFlash());
    }
}
