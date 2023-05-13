using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunStatTracker : MonoBehaviour
{

    public static PlayerRunStatTracker instance;

    private void Awake()
    {
        instance = this;
    }

    public int kills, jumps, shotsFired, damageDealt, runsCompleted, longestGodwalkerTime, damageTaken; 

}
