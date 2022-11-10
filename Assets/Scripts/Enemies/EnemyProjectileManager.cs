using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileManager : MonoBehaviour
{
    // this runs as a static instance which holds a list of all active enemy projectiles in the scene
    public static EnemyProjectileManager instance;
    private void Awake() => instance = this;
    // list of projectiles
    public List<GameObject> projectiles = new List<GameObject>();
    // function to kill all objects
    public void KillAllProjectiles()
    {
        // destroy them all
        for (int i = 0; i < projectiles.Count; i++)
        {
            Destroy(projectiles[i]);
            projectiles.Remove(projectiles[i]);
        }
    }

}
