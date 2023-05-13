using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathUIStatDisplay : MonoBehaviour
{
    public TextMeshProUGUI kills, jumps, shots, damage, damageTaken, runs, bestGod;

    // Start is called before the first frame update
    void Start()
    {

        // get instance
        PlayerRunStatTracker stats = PlayerRunStatTracker.instance;

        // set all values
        kills.text = "Kills: " + stats.kills;
        jumps.text = "Jumps: " + stats.jumps;
        shots.text = "Shots Fired: " + stats.shotsFired;
        damage.text = "Damage Dealt: " + stats.damageDealt;
        damageTaken.text = "Damage Taken: " + stats.damageTaken;
        runs.text = "Runs Completed: " + stats.runsCompleted;
        bestGod.text = "Best Godmode Time: " + stats.longestGodwalkerTime + " seconds";
    }

}
