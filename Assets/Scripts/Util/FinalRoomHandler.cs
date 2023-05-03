using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalRoomHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI kills, jumps, shots, damage, runs, bestGod;
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
        damage.text = "Damage Dealth: " + stats.damageDealt;
        runs.text = "Runs Completed: " + stats.runsCompleted;
        bestGod.text = "Best Godmode Time: " + stats.longestGodwalkerTime + " seconds";

        // cut the player's HP in half
        PlayerStatManager.instance.maxHealth /= 2;
    }
}
