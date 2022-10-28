using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBehaviour_PoolFire : EnemyBehaviour
{
    // this script is used to fire objects from a pool, and to build that pool

    [SerializeField] GameObject projectile; // which projectile are we firing?
    [SerializeField] Transform projectileOrigin; // the origin of our projectile
    [SerializeField] float fireAmount, fireRate; // how many are we firing and at what fire rate?

    [SerializeField] List<GameObject> pooledProjectiles = new List<GameObject>(); // our pool of projectiles

    private void Start()
    {
    }

    // our main coroutine
    public override IEnumerator MainCoroutine()
    {
        // loop and fire out our shots
        int fired = 0;
        while (fired < fireAmount)
        {
            // fire a projectile at the player
            GameObject shot = Spawn(projectile, projectileOrigin.position, projectileOrigin.rotation);
            // set the damage of the shot if this is a single shot
            if (shot.GetComponent<EnemyProjectile>())
            { shot.GetComponent<EnemyProjectile>().damage = enemyClass.damage; }
            else // if this is a group
            {
                foreach (Transform t in shot.transform)
                {
                    if (t.GetComponent<EnemyProjectile>())
                    {
                        t.GetComponent<EnemyProjectile>().damage = enemyClass.damage;
                    }
                }
            }

            // wait the fire rate
            yield return new WaitForSeconds(fireRate + Random.Range(0, fireRate));
            // advance
            fired++;
        }

        yield return null;
    }

    // grab an object from our pool
    GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (pooledProjectiles.Count > 0)
        {
            GameObject temp = null;
            // find an inactive projectile
            foreach (GameObject projectile in pooledProjectiles)
            {
                if (!projectile.activeInHierarchy)
                {
                    temp = projectile;
                }
            }

            // then if we got a temp, change position
            if (temp)
            {
                temp.transform.position = position;
                temp.transform.rotation = rotation;
                temp.SetActive(true);
                return temp;
            }

        }

        GameObject newSpawn = Instantiate(prefab, position, rotation);
        pooledProjectiles.Add(newSpawn); // add our new projetile to the list
        return newSpawn;
    }
} 
