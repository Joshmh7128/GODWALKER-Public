using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupClass : EnemyClassOld
{
    private void Start()
    {
        if (roomClass == null)
        {
            if (roomClass == null)
            { roomClass = GetComponentInParent<EnemyClassOld>().roomClass; }
        }
    }


    public override void TakeDamage(int dmg)
    {
        throw new System.NotImplementedException();
    }
}
