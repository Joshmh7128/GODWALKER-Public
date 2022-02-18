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

    [SerializeField] List<GameObject> waveParents; // the parents of wave objects that we'll put into our enemy lists
    [SerializeField] GameObject summonEffect; // our summoning particle effect
    bool isActive = false; // is this combat zone active? default to no
    int currentWave = 0; // which wave are we on?
    [SerializeField] int childCount = 0; // the amount of active children
    List<GameObject> activeParticles = new List<GameObject>(); // our list of active particles
    bool particlesActive; // are our particles active

    // starts our zone
    public void ActivateZone()
    {
        // activates our first wave
        ActivateCurrentWave();
    }

    void ActivateCurrentWave()
    {
        // for each child of the wave, set it to true
        foreach (Transform child in waveParents[currentWave].transform)
        {
            childCount++;
            child.gameObject.SetActive(true);
        }
    }

    void EndCombat()
    {
        // music change

        // open doors

        // environment lighting change
    }

    void ClearParticles()
    {
        if (activeParticles.Count > 0)
        {
            // get each particle and...
            foreach (GameObject gameObject in activeParticles)
            {   // ...perform a murder!
                Destroy(gameObject);
            }
        }

        // there is... no more...
        particlesActive = false;
    }

    // call whenever an enemy dies
    public void OnDeath()
    {
        // lower childcount
        childCount--;

        // check to see if our wave is ending...
        if (childCount == 0)
        {
            if (currentWave >= waveParents.Count)
            {
                EndCombat();
            }
            else 
            {
                Debug.Log("Wave " + currentWave + " over");
                currentWave++;
                ActivateCurrentWave();
                // clear our particles
                ClearParticles();
            }
        } else if (childCount <= 2 && !particlesActive && currentWave+1 < waveParents.Count) // make sure we can make our particles!
        {
            foreach (Transform child in waveParents[currentWave+1].transform)
            {
                // make the particle effect
                GameObject effect = Instantiate(summonEffect, child.position, Quaternion.identity, null);
                //add it to the list
                activeParticles.Add(effect);
            }

            // our particles are now active!!
            particlesActive = true;
        }
    }

}
