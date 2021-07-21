using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    // our list of map generation chunks
    [SerializeField] RandomChildSelector[] randomChildSelectors;

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
        yield return new WaitForSeconds(1f);
    }
}
