using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    /// <summary>
    /// this script runs the combat in rooms
    /// it activates the first wave of enemies, tracks how many are alive, and then starts the next one based on which wave
    /// the player is currently fighting
    /// </summary>

    [SerializeField] List<List<GameObject>> enemyWaves; // list of lists of enemies in each wave
    [SerializeField] GameObject summonEffect; // our summoning particle effect
    bool isActive = false; // is this combat zone active? default to no
    int currentWave = 0; // which wave are we on?
    List<GameObject> activeParticles; // our list of active particles

    // public function that allows us to activate a zone
    public void ActivateZone()
    {
        ActivateWave();
        isActive = true;
    }
    
    // use to clean all active particles
    void CleanParticles()
    { 
        foreach (GameObject particle in activeParticles)
        {
            Destroy(particle);
        }
    }

    void ActivateWave()
    {
        // activate our current wave
        foreach (GameObject enemy in enemyWaves[currentWave])
        {
            // activate them
            enemy.SetActive(true);
        }
    }

    void EndCombat()
    {
        // end combat
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if we are active
        if (isActive)
        {
            // cycle through the current enemyWave and make sure to remove any null enemy objects
            for (int i = enemyWaves[currentWave].Count - 1; i >= 0; i--)
            {
                // cleanup
                if (enemyWaves[currentWave][i] == null)
                { enemyWaves[currentWave].RemoveAt(i); }
            }

            // check how many enemies are alive in the current wave, if there are 2 or less alive of the current wave, 
            // spawn a particle effect at each position of each enemy in the next wave 
            if (enemyWaves[currentWave].Count <= 2)
            {
                bool particlesMade = false;

                if (particlesMade == false)
                {
                    foreach (GameObject enemy in enemyWaves[currentWave])
                    {
                        Instantiate(summonEffect, enemy.transform.position, Quaternion.identity, null);
                    }

                    particlesMade = true;
                }
            }

            // if all enemies in the current wave are killed activate the next wave and remove all the old particle effects
            if (enemyWaves[currentWave].Count == 0)
            {
                // next wave up
                if (enemyWaves[currentWave + 1] != null)
                {
                    currentWave++;
                    ActivateWave();
                    CleanParticles();
                }
                else if (enemyWaves[currentWave + 1] == null) 
                {
                    // end combat if there are no more waves
                    EndCombat();
                }
            }
        }
    }
}
