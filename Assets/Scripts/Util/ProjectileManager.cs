using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileManager
{
    // our list of projectiles
    public static List<EnemyBulletScript> projectiles = new List<EnemyBulletScript> ();
    // how many bullets can we have in a scene?
    public static int projectileLimit = 700;
    // function to remove the oldest bullet in the scene
    public static void CullCheck(int id)
    {
        // run a check to see if we need to cull
        if (projectiles.Count >= projectileLimit)
        {
            Debug.Log(projectiles.Count + " active projectiles");
            // destroy the projectiles relative to the end of the list
            Debug.Log("destroying at " + ((id+1) - projectileLimit));
            // make sure that position is not empty
            if ((id + 1) - projectileLimit >= 0)
            {
                if (projectiles[(id + 1) - projectileLimit] == null)
                { projectiles.RemoveAt((id + 1) - projectileLimit); }
                // then when it is not null, remove it
                if (projectiles[(id + 1) - projectileLimit] != null)
                { projectiles[(id + 1) - projectileLimit].DestroyBullet(); }
            }
        }
    }
}
