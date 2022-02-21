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
            // GameObject.Find("Player").GetComponent<PlayerController>().currentObjective.text = "Horde Vanquished. Prepare and continue to Boss.";
        }

        for(int i = 0; i < HordeEnemies.Count; i++)
        { if (HordeEnemies[i] == null)
            {
                HordeEnemies.RemoveAt(i);
            }
        }
    }
}
