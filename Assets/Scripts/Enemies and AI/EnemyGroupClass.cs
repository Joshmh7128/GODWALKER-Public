using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupClass : EnemyClass
{
    private void Start()
    {
        if (roomClass == null)
        {
            if (roomClass == null)
            { roomClass = GetComponentInParent<EnemyClass>().roomClass; }
        }
    }


    public override void TakeDamage(int dmg)
    {
        throw new System.NotImplementedException();
    }
}
