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
        int index = 0;
        while (index < startPositions.Count-1)
        {
            // set our projectile origin
            projectileOrigin = startPositions[index];
            // fire a projectile at the player
            GameObject shot = Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation);
            // set the damage of the shot
            shot.GetComponent<EnemyProjectile>().damage = enemyClass.damage;
            // wait the fire rate
            yield return new WaitForSeconds(fireRate + Random.Range(0, fireRate));
            // advance
            index++;
        }

        yield return null;

    }
}
