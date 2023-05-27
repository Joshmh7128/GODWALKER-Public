using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementGroup : MonoBehaviour
{
    public List<LevelutionElement> elements = new List<LevelutionElement>();
    
    List<GameObject> spawnPoints = new List<GameObject>();

    private void Awake()
    {
        // get all our children and add them to the elements list
        foreach (Transform child in transform)
        {
            elements.Add(child.GetComponent<LevelutionElement>());
            // sort and add spawn points
            if (child.GetComponent<EnemySpawnPoint>() != null) spawnPoints.Add(child);
        }

    }

    public void Evolve()
    {
        if (elements.Count > 0)
        {
            foreach (LevelutionElement element in elements)
                element.ActivateElement();

            foreach (GameObject spawnPoint in spawnPoints)
                spawnPoint.SetActive(true);
        }
    }
}
