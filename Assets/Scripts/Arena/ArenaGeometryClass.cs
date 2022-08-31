using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArenaGeometryClass : MonoBehaviour
{
    // this is for all our arena geometries, we use this to add spawn points
    [HideInInspector] public ArenaHandler handler; // our handler which we send spawnpoints to. this is set in the handler which creates this geometry
    [SerializeField] List<Transform> spawnPoints; // our spawnpoints

    // send out spawnpoints to the handler
    public void SendSpawnPoints()
    {
        handler.spawnPoints = spawnPoints;
    }

   
}
