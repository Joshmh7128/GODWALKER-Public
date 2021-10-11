using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChunkClusterSpawner : MonoBehaviour
{
    [SerializeField] int dropAmount;
    [SerializeField] GameObject smallChunk;

    // when enabled, spawn chunks
    private void OnEnable()
    {
        // spawn chunks based on the desired amount in a desired radius
        // choose a random amount of small chunks then spawn them in
        for (float i = 0; i < dropAmount; i++)
        {
            Instantiate(smallChunk, transform.position + new Vector3(Random.Range(-3, 3), transform.position.y + 1, Random.Range(-3, 3)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }
    }
}
