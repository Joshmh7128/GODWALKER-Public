using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Crab : EnemyClass
{
    [SerializeField] LocalIKHandler hurtHandler;

    // override our get hurt extend since we're not doing anything differently that requires an override of the main gethurt class
    public override void GetHurtExtension()
    {
        // kick our body
        hurtHandler.KickLookPos(30f);
    }
}
