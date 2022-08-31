using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceBaker : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] surfaces;   

    // when we are enabled, update the navigational mesh
    private void OnEnable()
    {
        UpdateNavMesh();
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        yield return new WaitForSeconds(1f);
        UpdateNavMesh();
    }
    // use this to update our nav mesh once we've built it
    public void UpdateNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}
