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
    public bool combatComplete = false; // is this combat zone complete? default to no
    int currentWave = 0; // which wave are we on?
    [SerializeField] int childCount = 0; // the amount of active children
    List<GameObject> activeParticles = new List<GameObject>(); // our list of active particles
    bool particlesActive; // are our particles active
    public List<DoorClass> doorClasses; // all the doors we will be locking and unlocking from combat
    [SerializeField] GameObject combatScenery, safeScenery; // our combat and safe scenery objects
    [SerializeField] bool devActivate;


    private void FixedUpdate()
    {
        if (devActivate)
        {
            ActivateZone();
        }
    }

    // starts our zone
    public void ActivateZone()
    {
        // activates our first wave
        ActivateCurrentWave();
        devActivate = false;
    }

    void ActivateCurrentWave()
    {
        // for each child of the wave, set it to true
        foreach (Transform child in waveParents[currentWave].transform)
        {
            childCount++;
            if (child)
            {
                child.gameObject.SetActive(true);
                // activate the enemyclass script on them if they have one
                child.GetComponent<EnemyClass>().isActive = true;
            }
        }

        // say our current wave to the player
        UpgradeSingleton.Instance.player.InteractableMessageTrigger(waveParents[currentWave].name, true);
        StartCoroutine(MessageClear());
    }


    IEnumerator MessageClear()
    {
        yield return new WaitForSeconds(5f);
        UpgradeSingleton.Instance.player.InteractableMessageTrigger(waveParents[currentWave].name, false);
    }

    void EndCombat()
    {
        // stop combat
        combatComplete = true;
        // music change
        FindObjectOfType<MusicController>().MusicMood(MusicController.musicMoods.explore);
        // unlock doors
        foreach (DoorClass door in doorClasses)
        { door.Unlock(); }
        // environment lighting change
        combatScenery.SetActive(false);
        safeScenery.SetActive(true);
        // tell the player we are complete
        UpgradeSingleton.Instance.player.InteractableMessageTrigger("Room Clear", true);
        StartCoroutine(MessageClear());
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
            if (currentWave >= waveParents.Count-1)
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
                GameObject effect = Instantiate(summonEffect, child.position + new Vector3(0f, 2f, 0f), Quaternion.identity, null);
                //add it to the list
                activeParticles.Add(effect);
            }

            // our particles are now active!!
            particlesActive = true;
        }
    }

}
