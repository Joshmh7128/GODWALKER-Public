using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public abstract void TakeDamage(int dmg, Vector3 dmgOrigin);
}
