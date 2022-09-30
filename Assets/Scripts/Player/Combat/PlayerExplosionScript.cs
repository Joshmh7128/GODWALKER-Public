using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionScript : MonoBehaviour
{
    /// this script goes on every single explosion that the player can spawn
    /// 

    PlayerBodyPartManager bodyPartManager;
    public float damage; // set by our bullet when we are instantiated
    int enemiesHit; // how many enemies this explosion hit

    // get instance
    private void Awake() => bodyPartManager = PlayerBodyPartManager.instance;

    // every frame get all the colliders
    private void OnTriggerStay(Collider other)
    {
        // check everything we're colliding with - is there an enemy?
        if (other.gameObject.transform.tag == "Enemy")
        {
            enemiesHit++;
            // call our explosion damage
            bodyPartManager.CallParts("OnExplosionDamage");
            // deal damage
            other.gameObject.GetComponent<EnemyClass>().GetHurt(damage);
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
        if (enemiesHit > 1) bodyPartManager.CallParts("OnMultipleExplosionDamage");
        // then disable collider
        gameObject.GetComponent<Collider>().enabled = false;
        // then wait another 2 seconds
        yield return new WaitForSecondsRealtime(2f);
        // then destroy this object
        Destroy(gameObject);
    }

}
