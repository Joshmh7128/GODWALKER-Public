using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public abstract void TakeDamage(int dmg, Vector3 dmgOrigin);
    public bool invincible = false; // are we invincible?

    public void AddToManager()
    {
        GameObject.Find("Enemy Manager").GetComponent<EnemyManager>().enemies.Add(gameObject);
    }
}
