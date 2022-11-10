using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_ProjectileFire : EnemyBehaviour
{
    [SerializeField] GameObject projectile; // which projectile are we firing?
    [SerializeField] Transform projectileOrigin; // the origin of our projectile
    [SerializeField] float fireAmount, fireRate; // how many are we firing and at what fire rate?
    [SerializeField] float overrideSpeed; // are we overriding the speed of the shot?
    // our main coroutine
    public override IEnumerator MainCoroutine()
    {
        // loop and fire out our shots
        int fired = 0;
        while (fired < fireAmount)
        {
            GameObject shot = null;
            // fire a projectile at the player
            try { shot = Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation); } catch { break; };
            // set the damage of the shot if this is a single shot
            if (shot.GetComponent<EnemyProjectile>())
            {  // shot.GetComponent<EnemyProjectile>().damage = enemyClass.damage;
             }
            else // if this is a group
            {
                foreach (Transform t in shot.transform)
                {
                    if (t.GetComponent<EnemyProjectile>())
                    {
                        // t.GetComponent<EnemyProjectile>().damage = enemyClass.damage;
                        if (overrideSpeed != 0)
                        {
                            t.GetComponent<EnemyProjectile>().speed = overrideSpeed;
                        }
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
}
