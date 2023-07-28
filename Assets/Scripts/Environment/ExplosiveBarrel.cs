using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : EnemyClass
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, OnDeathFX.transform.localScale.x);
    }
}
