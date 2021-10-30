using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshModification : MonoBehaviour
{
    // this script will be used to modify all of our mesh data
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] outVertices;
    int[] outTriangles;
    [SerializeField] float randomRange;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] +=  new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
        }

        mesh.vertices = vertices;

        RemakeMeshToDiscrete(mesh.vertices, mesh.triangles);

        mesh.vertices = outVertices;
        mesh.triangles = outTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        RemakeMeshToDiscrete(mesh.vertices, mesh.triangles);

    }


    void RemakeMeshToDiscrete(Vector3[] vert, int[] trig)
    {
        /*
        vert - vertices of smooth mesh
        trig - triangles of smooth mesh

        outVertices - vertices of edged mesh
        outTriangles - triangles of edged mesh
        */

        Vector3[] vertDiscrete = new Vector3[trig.Length];
        int[] trigDiscrete = new int[trig.Length];
        for (int i = 0; i < trig.Length; i++)
        {
            vertDiscrete[i] = vert[trig[i]];
            trigDiscrete[i] = i;
        }
        outVertices = vertDiscrete;
        outTriangles = trigDiscrete;
    }
}
