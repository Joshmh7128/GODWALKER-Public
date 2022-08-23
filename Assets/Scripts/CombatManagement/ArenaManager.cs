using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    /// <summary>
    /// This script manages one combat arena
    /// It enables enemies in order from it's inactive pool, and continues until there are none left
    /// then it spawns a reward (?)
    /// </summary>
    /// 

    [SerializeField] Transform inactiveParent, activeParent; // our parents for inactive and active enemies
    [SerializeField] GameObject summoningEffect; // the visual effect for where an enemy will be summoned

    [SerializeField] int activeGoal; // how many do we want active at once?

    private void FixedUpdate()
    {
        ProcessEnemyAmount();
    }

    // process our enemies
    void ProcessEnemyAmount()
    {
        if (activeParent.childCount < activeGoal)
        {
            if (inactiveParent.GetChild(0))
            {
                Transform child = inactiveParent.GetChild(0);
                child.parent = activeParent;
                child.gameObject.SetActive(true);
            }
        }
    }
}
