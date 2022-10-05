using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PlayerShockExplosionScript : MonoBehaviour
{
    /// this script goes on every single explosion that the player can spawn
    /// 

    PlayerBodyPartManager bodyPartManager;
    PlayerProjectileManager projectileManager;
    PlayerWeaponManager weaponManager;
    [HideInInspector] public float damage; // set by our bullet when we are instantiated
    public int enemiesHit; // how many enemies this explosion hit
    [SerializeField] DamageNumber shockHit;
    public bool used; // has this been used in an effect already?
    [SerializeField] bool doesLoop; // does this loop?
    // get instance
    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;
        weaponManager = PlayerWeaponManager.instance;

        // immediately add ourselves to the projectile manager's list of explosions
        if (!doesLoop)
        projectileManager.shockExplosionScripts.Add(this);
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
            damage = weaponManager.currentWeapon.damage *= Random.Range(1.1f, 1.8f);
            // deal damage         
            other.GetComponent<EnemyClass>().GetHurt(damage);
            // apply effect
            other.GetComponent<EnemyClass>().ApplyEffect(EnemyClass.Effects.Shock);

            // spawn normal damage number
            shockHit.Spawn(transform.position, damage);
            // disable the collider in the next frame
            StartCoroutine(DisableBuffer()); // then disable the collider
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
        // check how many enemies we hit
        bodyPartManager.CallParts("OnShockDamage");
        // wait for the fixed update
        yield return new WaitForFixedUpdate();
        // then remove the explosion from the list
        if (!doesLoop)
            projectileManager.shockExplosionScripts.Remove(this);
        // then disable collider
        gameObject.GetComponent<Collider>().enabled = false;
        // then wait another 2 seconds
        yield return new WaitForSecondsRealtime(1f);
        // then destroy this object
        if (!doesLoop)
            Destroy(gameObject);
        if (doesLoop)
            OverlapCheck();
    }

}
