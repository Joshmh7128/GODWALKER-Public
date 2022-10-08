using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_HelionProjectileFire : EnemyBehaviour
{
    [SerializeField] GameObject projectile; // which projectile are we firing?
    [SerializeField] Transform projectileOrigin; // the origin of our projectile
    [SerializeField] float fireRate; // how many are we firing and at what fire rate?

    [SerializeField] List<Transform> startPositions; // shot start positions (set in editor)

    // our main coroutine
    public override IEnumerator MainCoroutine()
    {
        // loop and fire out our shots
        if (!complete)
        {
            int index = 0;
            while (index < startPositions.Count - 1)
            {
                if (!complete) // if we can still run this
                {
                    // set our projectile origin
                    projectileOrigin = startPositions[index];
                    // fire a projectile away from the center of mass
                    Vector3 dir = projectileOrigin.position - enemyClass.transform.position;
                    Quaternion q = Quaternion.LookRotation(dir);
                    GameObject shot = Instantiate(projectile, projectileOrigin.position, q);
                    // set the damage of the shot
                    shot.GetComponent<EnemyProjectile>().damage = enemyClass.damage;
                    // wait the fire rate
                    yield return new WaitForSeconds(fireRate);
                    // advance
                    index++;
                }
            }

            // at the end check if we are not complete and start over
            if (!complete) StartCoroutine(MainCoroutine());

        }
        yield return null;

    }
}
