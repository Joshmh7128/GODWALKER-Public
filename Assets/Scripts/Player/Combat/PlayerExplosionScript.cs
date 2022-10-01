using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PlayerExplosionScript : MonoBehaviour
{
    /// this script goes on every single explosion that the player can spawn
    /// 

    PlayerBodyPartManager bodyPartManager;
    PlayerProjectileManager projectileManager;
    [HideInInspector] public float damage; // set by our bullet when we are instantiated
    public int enemiesHit; // how many enemies this explosion hit
    [SerializeField] DamageNumber explosionHit;
    // get instance
    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;

        // immediately add ourselves to the projectile manager's list of explosions
        projectileManager.explosionScripts.Add(this);
    }

    // every frame get all the colliders
    private void OnTriggerStay(Collider other)
    {
        // check everything we're colliding with - is there an enemy?
        if (other.gameObject.transform.tag == "Enemy")
        {
            enemiesHit++;
            // deal damage
            other.gameObject.GetComponent<EnemyClass>().GetHurt(damage);
            // random normal modifier
            damage *= Random.Range(1.1f, 1.8f);
            // spawn normal damage number
            explosionHit.Spawn(transform.position, damage);
            // disable the collider in the next frame
            StartCoroutine(DisableBuffer()); // then disable the collider
        }
    }

    // a buffer to disable our collider and destroy our object
    IEnumerator DisableBuffer()
    {
        // wait for the fixed update
        yield return new WaitForFixedUpdate();
        // check how many enemies we hit
        bodyPartManager.CallParts("OnExplosionDamage");
        if (enemiesHit > 1) bodyPartManager.CallParts("OnMultipleExplosionDamage");
        // wait for the fixed update
        yield return new WaitForFixedUpdate();
        // then remove the explosion from the list
        projectileManager.explosionScripts.Remove(this);
        // then disable collider
        gameObject.GetComponent<Collider>().enabled = false;
        // then wait another 2 seconds
        yield return new WaitForSecondsRealtime(2f);
        // then destroy this object
        Destroy(gameObject);
    }

}
