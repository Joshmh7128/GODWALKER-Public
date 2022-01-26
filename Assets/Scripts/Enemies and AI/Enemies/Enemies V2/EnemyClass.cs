using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public float HP; // our health points
    bool invinicble; // are we invincible right now?
    public abstract void TakeDamage(int dmg); // how much damage are we taking?
}
