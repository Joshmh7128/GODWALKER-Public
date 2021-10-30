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
    [SerializeField] bool canChange;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (vertices[i] == mesh.vertices[i])
            vertices[i] +=  new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
           
        }

        //mesh.vertices = vertices;
        mesh.SetVertices(vertices);

        RemakeMeshToDiscrete(mesh.vertices, mesh.triangles);

        mesh.vertices = outVertices;
        mesh.triangles = outTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

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
