using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementGroup : MonoBehaviour
{
    public List<LevelutionElement> elements = new List<LevelutionElement>();

    [SerializeField] bool manualActivation;

    List<GameObject> spawnPoints = new List<GameObject>();

    private void Awake()
    {
        // get all our children and add them to the elements list
        foreach (Transform child in transform)
        {
            if (child.GetComponent<LevelutionElement>() != null)
                elements.Add(child.GetComponent<LevelutionElement>());
            // sort and add spawn points
            if (child.GetComponent<EnemySpawnPoint>() != null)
            {
                // turn off the child
                child.gameObject.SetActive(false);
                // add it to the list
                spawnPoints.Add(child.gameObject);
            }

            // if this thing doesn't have a spawn point or a levelution element, activate it
            if (child.GetComponent<EnemySpawnPoint>() == null && child.GetComponent<LevelutionElement>() == null)
                child.gameObject.SetActive(true);

        }
    }

    private void Start()
    {
        // for testing 
        if (manualActivation) Evolve();
    }

    public void Evolve()
    {
        if (elements.Count > 0)
        {
            foreach (LevelutionElement element in elements)
                element.ActivateElement();

            foreach (GameObject spawnPoint in spawnPoints)
                spawnPoint.transform.parent = FindObjectOfType<ArenaHandler>().spawnPointParent;
        }
    }
}
