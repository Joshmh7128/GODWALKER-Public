using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactSetInstantiationScript : MonoBehaviour
{
    [SerializeField] List<GameObject> artifactPrefabs;

    // script serves to spawn one of the many upgrade prefabs we have
    void Start()
    {
        Instantiate(artifactPrefabs[(int)Random.Range(0f, (float)artifactPrefabs.Count)], transform);
    }
}
