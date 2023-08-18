using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalRoomHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI kills, jumps, shots, damage, runs, bestGod, damageTaken, godDamageTaken;
    // Start is called before the first frame update
    void Start()
    {
        // advance the run
        PlayerRunStatTracker.instance.runsCompleted += 1;

        // get instance
        PlayerRunStatTracker stats = PlayerRunStatTracker.instance;

        // set all values
        kills.text = "Kills: " + stats.kills;
        jumps.text = "Jumps: " + stats.jumps;
        shots.text = "Shots Fired: " + stats.shotsFired;
        damage.text = "Damage Dealt: " + stats.damageDealt;
        damageTaken.text = "Damage Taken Normalwalking: " + stats.damageTaken;
        godDamageTaken.text = "Damage Taken In Godmode: " + stats.godwalkerDamageTaken;
        runs.text = "Runs Completed: " + stats.runsCompleted;
        bestGod.text = "Best Godmode Time: " + stats.longestGodwalkerTime + " seconds";

        // multiply our damage multiplier by 2
        PlayerStatManager.instance.damageMultiplier *= 2;
    }
}
