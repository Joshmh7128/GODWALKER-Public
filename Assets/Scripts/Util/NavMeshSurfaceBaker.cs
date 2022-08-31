using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceBaker : MonoBehaviour
{
    NavMeshSurface surface;   

    // when we are enabled, update the navigational mesh
    private void OnEnable()
    {
        surface = GetComponent<NavMeshSurface>();
        UpdateNavMesh();
    }

    // use this to update our nav mesh once we've built it
    public void UpdateNavMesh()
    {
        surface.BuildNavMesh();
    }
}
