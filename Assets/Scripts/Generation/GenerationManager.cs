using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    // our list of map generation chunks
    [SerializeField] RandomChildSelector[] randomChildSelectors;
    // bug parent
    [SerializeField] Transform enemyManager;

    public void MapRegen()
    {
        StartCoroutine("MapRegenCo");
    }

    // trigger regeneration
    IEnumerator MapRegenCo()
    {
        // generate while drop pod is up in the air before it comes down
        foreach (RandomChildSelector selector in randomChildSelectors)
        {
            selector.Regen();
        }

        // kill all the enemies
        foreach (Transform ourGameObject in enemyManager)
        {
            Destroy(ourGameObject.gameObject);
        }

        yield return new WaitForSeconds(1f);
    }
}
