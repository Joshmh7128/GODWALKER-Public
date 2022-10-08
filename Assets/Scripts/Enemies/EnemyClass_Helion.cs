using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass_Helion : EnemyClass
{
    /// the helion is a huge enemy that fires noisy shots out of the tips all over its body
    /// it alternates between noise mode (orange) and homing mode (purple)
    [SerializeField] float baseHealth; // health we set in-editor, this is a beefier enemy so we want it to be higher

    // set our stats
    public override void SetLevelStats()
    {
        // set our health
        maxHealth = (level + level * 0.15f) * baseHealth; // crabs have 100hp per level
        health = maxHealth;
        // set our damage
        // damage = level * (2 + level * 0.1f); // this is a standard curve
        damage = 10; // set our basic damage
    }

    public void FixedUpdate()
    {
        // rotate our body slowly to make us look more dramatic
        transform.eulerAngles += new Vector3(0, 0.15f, 0);
    }


}
