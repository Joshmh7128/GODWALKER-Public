using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerRunStatTracker : MonoBehaviour
{

    public static PlayerRunStatTracker instance;

    // telemetry variables
    public DateTime startTime;

    private void Awake()
    {
        instance = this;
        startTime = DateTime.Now;
    }

    public int kills, jumps, shotsFired, damageDealt, runsCompleted, longestGodwalkerTime, damageTaken, godwalkerDamageTaken, roomsCompleted; 

    // all of the weapons the player has used this run
    public Dictionary<string, int> weaponUsage = new Dictionary<string, int>();


}
