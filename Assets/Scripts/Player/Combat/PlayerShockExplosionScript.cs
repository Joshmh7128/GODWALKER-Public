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
    public bool doesLoop; // does this loop?

    /// links / tethers
    [SerializeField] LineRenderer lineRenderer; // our line renderer for connecting looping explosion zones together
    public bool isTethered; // is this tethered?
    public Transform tetherTarget; // our target for tethering
    public Transform lineRendStart, lineRendEnd; // for our line renderer start and end positions

    // a prefab of ourselves
    [SerializeField] GameObject shockExplosion;

    // get instance
    private void Awake()
    {
        projectileManager = PlayerProjectileManager.instance;
        bodyPartManager = PlayerBodyPartManager.instance;
        weaponManager = PlayerWeaponManager.instance;

        // immediately add ourselves to the projectile manager's list of explosions
        if (!doesLoop)
            projectileManager.shockExplosionScripts.Add(this);
        if (doesLoop)
            projectileManager.loopingShockExplosionScripts.Add(this);
    }

    private void Start()
    {
        // check for our explosion
        OverlapCheck();
    }

    private void FixedUpdate()
    {
        // process our line renderer
        if (isTethered)
        ProcessTether();
    }

    void CheckAction(GameObject other)
    {
        // check everything we're colliding with - is there an enemy?
        if (other.transform.tag == "Enemy")
        {
            enemiesHit++;
            // random normal modifier
            damage = weaponManager.currentWeapon.damage * Random.Range(0.1f, 0.15f);
            // deal damage         
            other.GetComponent<EnemyClass>().GetHurt(damage);
            // apply effect
            other.GetComponent<EnemyClass>().ApplyEffect(EnemyClass.Effects.Shock);

            // spawn normal damage number
            shockHit.Spawn(transform.position, damage);
            // disable the collider in the next frame
            StartCoroutine(DisableBuffer()); // then disable the collider
        }
   
        // if this is interactable, try and interact with it
        if (other.transform.tag == "Interactable")
        {
            Debug.Log("interactable hit");

            try { other.GetComponent<BatteryHandler>().ChargeBattery(); } catch { }
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
        {
            projectileManager.shockExplosionScripts.Remove(this);
            // then disable collider
            gameObject.GetComponent<Collider>().enabled = false;
        }
        // then wait another 2 seconds
        yield return new WaitForSecondsRealtime(1f);
        // then destroy this object
        if (!doesLoop)
            Destroy(gameObject);
        if (doesLoop)
            OverlapCheck();
    }

    // our function for linking tethers
    public void BuildTether()
    {
        if (doesLoop)
        {
            // refresh our shock list
            projectileManager.RefreshShockList();
            // find another active shock explosion and tether to it
            foreach (var shockEx in projectileManager.loopingShockExplosionScripts)
            {
                if (shockEx != this && !shockEx.isTethered && shockEx.doesLoop)
                {
                    // set our line renderer to follow our position and follow the other shock explosion's position
                    isTethered = true;

                    lineRendStart = transform;
                    lineRendEnd = shockEx.transform;

                    // then tell our end to tether!
                    shockEx.BuildTether();

                    break; // break out of the loop
                }
            }
        }
    }

    // run when we want to update our line renderer positions
    void ProcessTether()
    {
        lineRenderer.SetPosition(0, lineRendStart.position);

        if (lineRendEnd != null)
        {
            lineRenderer.SetPosition(1, lineRendEnd.position);
            // if we have a start and end point, perform a linecast to those points, and spawn an explosion if we hit an enemy
            RaycastHit hit; Physics.Linecast(lineRendStart.position, lineRendEnd.position, out hit, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            if (hit.transform.tag == "Enemy")
            {
                if (!hit.transform.gameObject.GetComponent<EnemyClass>().activeEffects.Contains(EnemyClass.Effects.Shock))
                Instantiate(shockExplosion, hit.transform.position, Quaternion.identity, null);
            }
        }

        // check if we lost our end
        if (lineRendEnd == null)
        { lineRendEnd = transform; }
    }
}
