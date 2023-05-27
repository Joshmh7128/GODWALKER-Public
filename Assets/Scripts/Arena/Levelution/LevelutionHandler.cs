using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionHandler : MonoBehaviour
{
    // this handler receives a call from the ArenaHandler whenever a wave of enemies is completed
    // it then continues the evolution of the level by activatng element groups

    // our groups
    [SerializeField] List<LevelutionElementGroup> elementGroups;
    
    // our call action to evolve the level
    public void Evolve()
    {
        // choose a random element group and evolve
        int i = Random.Range(0, elementGroups.Count);
        elementGroups[i].Evolve();
        elementGroups.Remove(elementGroups[i]);
    }
}
