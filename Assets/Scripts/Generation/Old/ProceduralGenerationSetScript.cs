using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerationSetScript : MonoBehaviour
{
    // managed the generation of procedural chunks

    // our list of gameobject to be spawned
    [SerializeField] List<GameObject> chunkSets;

    // our OnGenerate() function
    public void OnGenerate()
    {
        // spawn a random one
        Instantiate(chunkSets[Random.Range(0, chunkSets.Count)], transform);
    }
}
