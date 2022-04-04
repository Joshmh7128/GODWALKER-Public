using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    public float HP, maxHP; // our health points
    public string NameText;
    bool invincible; // are we invincible right now?
    public bool isActive; // is this enemy active?
    public abstract void TakeDamage(int dmg); // how much damage are we 
    public abstract void OnDeath(); // our death function
    public abstract void Activate(); // activation
    public abstract void Attack(); // our attack
    public enum enemyTypes
    { ground, flying }
    public enemyTypes enemyType;

    public enum dropTypes // what can we drop?
    {
        none,
        nanites,
        power,
        HP
    }

    public CombatZone combatZone; // what combat zone are we in?

    // use our awake function to check the parent of our parent 
    public void Awake()
    {
        // if we don't have a combat zone, get it from our root parent
        if (!combatZone)
            combatZone = transform.root.GetComponent<CombatZone>();
    }



}
