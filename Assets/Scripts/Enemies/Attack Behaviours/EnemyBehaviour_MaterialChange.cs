using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_MaterialChange : EnemyBehaviour
{
    // all materials we want to modify
    [SerializeField] List<Renderer> renderers;
    [SerializeField] Material material;

    public override IEnumerator MainCoroutine()
    {
        // change all the materials in our render list
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
        yield return new WaitForSecondsRealtime(behaviourTime);
    }
}
