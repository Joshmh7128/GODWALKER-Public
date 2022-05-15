using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicVisibilityCull : MonoBehaviour
{
    Renderer localRenderer; // our renderer

    private void Start()
    {
        // get our renderer
        localRenderer = gameObject.GetComponent<Renderer>();
    }

    private void OnBecameVisible()
    {
        localRenderer.enabled = true;
    }

    private void OnBecameInvisible()
    {
        localRenderer.enabled = false;
    }
}
