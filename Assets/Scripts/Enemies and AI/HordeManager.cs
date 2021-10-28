using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeManager : MonoBehaviour
{
    // our enemy list
    [SerializeField] List<GameObject> HordeEnemies;

    private void FixedUpdate()
    {
        if (HordeEnemies.Count < 1)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().currentObjective.text = "Objective Complete. Return to Dropship.";
        }
    }
}
