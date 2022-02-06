using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public float HP, maxHP; // our health points
    public string NameText;
    bool invincible; // are we invincible right now?
    public abstract void TakeDamage(int dmg); // how much damage are we taking?
    public enum dropTypes // what can we drop?
    {
        none,
        nanites,
        power,
        HP
    }
}
