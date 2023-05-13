using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerRunStatTracker : MonoBehaviour
{

    public static PlayerRunStatTracker instance;

    private void Awake()
    {
        instance = this;
        startTime = Time.realtimeSinceStartup;
    }

    public int kills, jumps, shotsFired, damageDealt, runsCompleted, longestGodwalkerTime, damageTaken, godwalkerDamageTaken; 

    // telemetry variables
    public float startTime;

    // all of the weapons the player has used this run
    public Dictionary<string, int> weaponUsage;


}
