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

    [SerializeField] Transform activeParent, inactiveParent; // our parents for inactive and active enemies
    [SerializeField] GameObject summoningEffect; // the visual effect for where an enemy will be summoned
    GameObject previousSummon; // our previous summon
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
                EnableNewEnemy();
            }
        }
    }

    void EnableNewEnemy()
    {
        Transform child = inactiveParent.GetChild(0);
        child.parent = activeParent;
        child.gameObject.SetActive(true);
        // spawn our summoningEffect where the new enemy will start
        if (previousSummon) { Destroy(previousSummon); }
        if (inactiveParent.childCount > 1)
        previousSummon = Instantiate(summoningEffect, inactiveParent.GetChild(0).position, Quaternion.identity, null);
    }
}
