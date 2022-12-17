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
    public float damage; // set by our bullet when we are instantiated
    public int enemiesHit; // how many enemies this explosion hit
    // [SerializeField] DamageNumber explosionHit;
    public bool used; // has this been used in an effect already?
    // get instance
    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;

        // immediately add ourselves to the projectile manager's list of explosions
        projectileManager.explosionScripts.Add(this);
    }

    private void Start()
    {
        // check for our explosion
        OverlapCheck();
    }

    void CheckAction(GameObject other)
    {
        // check everything we're colliding with - is there an enemy?
        if (other.transform.tag == "Enemy")
        {
            enemiesHit++;
            // random normal modifier
            damage *= Random.Range(0.1f, 0.15f);
            // deal damage
            other.GetComponent<EnemyClass>().GetHurt(damage);
            // spawn normal damage number
            // explosionHit.Spawn(transform.position, damage);
            // disable the collider in the next frame
            StartCoroutine(DisableBuffer()); // then disable the collider
        }

        // if we collide with the player, deal damage to them
        if (other.transform.tag == "Player")
        {
            PlayerStatManager.instance.TakeDamage(damage);
            bodyPartManager.CallParts("OnExplosionDamagePlayer"); // damage the player
        }
    }

    private void OverlapCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x);
        foreach (var collider in colliders)
        {
            CheckAction(collider.gameObject);
        }
    }


    // a buffer to disable our collider and destroy our object
    IEnumerator DisableBuffer()
    {
        // wait for the fixed update
        yield return new WaitForFixedUpdate();
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
