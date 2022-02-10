using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrassController : MonoBehaviour
{
    static readonly int _Positions = Shader.PropertyToID("_Positions");
    static readonly int _Normals = Shader.PropertyToID("_Normals");
    static readonly int _NormalsLength = Shader.PropertyToID("_NormalsLength");
    static readonly int _MeshPosition = Shader.PropertyToID("_MeshPosition");
    static readonly int _Angle = Shader.PropertyToID("_Angle");
    static readonly int _CameraForward = Shader.PropertyToID("_CameraForward");
    static readonly int _ColorA = Shader.PropertyToID("_ColorA");
    static readonly int _ColorB = Shader.PropertyToID("_ColorB");
    static readonly int _GrassHeight = Shader.PropertyToID("_GrassHeight");
    static readonly int _GrassWidth = Shader.PropertyToID("_GrassWidth");
    static readonly int _HeightMap = Shader.PropertyToID("_HeightMap");

    // Number of random points to generate per triangle on the mesh
    public const int GrassPointsPerTriangle = 300;

    public Material grassMaterialAsset;
    Material material;

    [Space(10)]
    public float minWidth;
    public float maxWidth;
    public float minHeight;
    public float maxHeight;

    public Color colorA;
    public Color colorB;

    public Texture2D[] heightMaps;

    [Space(10)]
    [Range(0f, 1f)] public float tallGrassChance;
    public float minTallgrassWidth;
    public float maxTallgrassWidth;
    public float minTallgrassHeight;
    public float maxTallgrassHeight;

    // Some height maps don't work as well with tall grass (patchy maps are better)
    public Texture2D[] tallGrassHeightMaps;

    [Space(10)]
    public MeshFilter grassyObject;
    Mesh mesh;

    readonly List<Vector3> grassPositions = new List<Vector3>(32768);
    ComputeBuffer positionsBuffer;

    Vector3[] normals;
    readonly List<Vector3> grassNormals = new List<Vector3>(32000);
    ComputeBuffer normalsBuffer;

    Vector3[] vertices;
    int[] triangles;
    int numTriangles;

    Bounds bounds;
    Vector3 boundsCenter;

    // You can use another random here - but it needs to be DETERMINISTIC so that the grass blades generate in the same spots
    float random;

    int grassIterations;

    void Start()
    {
        SetMesh(grassyObject);
    }

    void Update()
    {
        AddNextGrassPoints();
        DrawGrass();
    }

    void OnDestroy()
    {
        Destroy(material);

        if (positionsBuffer != null)
        {
            positionsBuffer.Release();
            positionsBuffer.Dispose();
            normalsBuffer.Release();
            normalsBuffer.Dispose();
        }
    }

    public void SetMesh(MeshFilter grassyObject)
    {
        this.grassyObject = grassyObject;
        mesh = grassyObject.sharedMesh;

        if (!material)
        {
            material = Material.Instantiate(grassMaterialAsset);
        }

        grassPositions.Clear();
        grassNormals.Clear();

        random = Random.Range(0,111);

        vertices = mesh.vertices;
        triangles = mesh.triangles;
        numTriangles = mesh.triangles.Length - 2;
        normals = mesh.normals;
        bounds = mesh.bounds;
        boundsCenter = bounds.center;

        grassIterations = 0;

        // Release previously used buffers
        if (positionsBuffer != null)
        {
            positionsBuffer.Release();
            positionsBuffer.Dispose();
            normalsBuffer.Release();
            normalsBuffer.Dispose();
        }

        // Create position buffer with a size of the max number of grass blades for this object (for data type size we can't do sizeof(Vector3) so we can just use 3 floats
        positionsBuffer = new ComputeBuffer(numTriangles / 3 * (GrassPointsPerTriangle + 1), sizeof(float) * 3);
        material.SetBuffer(_Positions, positionsBuffer);

        normalsBuffer = new ComputeBuffer(numTriangles / 3, sizeof(float) * 3);
        material.SetBuffer(_Normals, normalsBuffer);

        material.SetInt(_NormalsLength, numTriangles / 3);
        material.SetColor(_ColorA, colorA);
        material.SetColor(_ColorB, colorB);

        if (Random.Range(0,1) <= tallGrassChance)
        {
            material.SetFloat(_GrassWidth, Random.Range(minTallgrassWidth, maxTallgrassWidth));
            material.SetFloat(_GrassHeight, Random.Range(minTallgrassHeight, maxTallgrassHeight));
            material.SetTexture(_HeightMap, tallGrassHeightMaps[Random.Range(0, tallGrassHeightMaps.Length+1)]);
        }
        else
        {
            material.SetFloat(_GrassWidth, Random.Range(minWidth, maxWidth));
            material.SetFloat(_GrassHeight, Random.Range(minHeight, maxHeight));
            material.SetTexture(_HeightMap, heightMaps[Random.Range(0, heightMaps.Length)]);
        }
    }

    /// <summary>
    /// Generate one new blade per triangle each frame - Generating all at once causes a lag spike, so prbly fine to do it on Start but not if we're generating grass during gameplay
    /// </summary>
    public void AddNextGrassPoints()
    {
        if (grassIterations > GrassPointsPerTriangle)
        {
            return;
        }

        bool firstIteration = grassIterations == 0;

        for (int i = 0; i < numTriangles / 3; i++)
        {
            int index = i * 3;
            if (index >= numTriangles)
            {
                return; // Maybe don't need this I don't remeber why it's here
            }

            // Get the three vertices that make up this triangle
            ref Vector3 a = ref vertices[triangles[index]];
            ref Vector3 b = ref vertices[triangles[index + 1]];
            ref Vector3 c = ref vertices[triangles[index + 2]];

            // Generate a random point inside the triangle (I'm using a fast approximate square root but a normal Mathf.Sqrt would work too
            float r1Sqrt = Mathf.Sqrt(Random.Range(0f, 1f));
            float r2 = Random.Range(0f, 1f);
            Vector3 randPoint = (1f - r1Sqrt) * a + (r1Sqrt * (1f - r2)) * b + (r2 * r1Sqrt) * c;

            grassPositions.Add(randPoint);

            // Only set grass normal on our first time through the mesh
            if (firstIteration)
            {
                grassNormals.Add(normals[triangles[index]]);
            }
        }

        // We have to set the buffer data again each time we add new points
        positionsBuffer.SetData(grassPositions);

        if (firstIteration)
        {
            normalsBuffer.SetData(grassNormals);
        }

        ++grassIterations;
    }

    /// <summary>
    /// Dispatch grass shader
    /// </summary>
    public void DrawGrass()
    {
        // Only using Camera.main for testing
        Camera camera = FindObjectOfType<Camera>();

        Shader.SetGlobalVector(_Angle, camera.transform.eulerAngles);
        Shader.SetGlobalVector(_CameraForward, camera.transform.forward);

        material.SetVector(_MeshPosition, grassyObject.transform.position);

        bounds.center = boundsCenter + grassyObject.transform.position;
        Graphics.DrawProcedural(material, bounds, MeshTopology.Triangles, 3, grassPositions.Count);
    }
}