using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_ProjectileFire : EnemyBehaviour
{
    [SerializeField] GameObject projectile; // which projectile are we firing?
    [SerializeField] Transform projectileOrigin; // the origin of our projectile
    [SerializeField] float fireAmount, fireRate; // how many are we firing and at what fire rate?
    public float damage; // what is our damage?
    // our main coroutine
    public override IEnumerator MainCoroutine()
    {
        damage = enemyClass.damage;

        int fired = 0;

        while (fired < fireAmount)
        {
            // fire a projectile at the player
            Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation);
            // wait the fire rate
            yield return new WaitForSeconds(fireRate + Random.Range(0, fireRate));
            // advance
            fired++;
        }

        yield return null;

    }
}
