using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScriptFX : MonoBehaviour
{
    // variables
    public Transform bulletTarget; // what is the target of our bullet?
    public float bulletDamage; // what is the damage of our bullet
    [SerializeField] float bulletSpeed; // what is the speed of our bullet?
    [SerializeField] GameObject cubePuff; // our particle effect on death
    [SerializeField] GameObject currentParticleEffect; // the particle effect that emits during flight
    public enum bulletTypes
    {
        Projectile,
        Hitscan
    }

    public bulletTypes bulletType;

    // for when our bullet is instantiated
    private void Start()
    {
        // turn bullet
        transform.LookAt(bulletTarget);
   
        // start the safety kill
        StartCoroutine("SafetyKill");
    }

    // Update is called once per frame
    void Update()
    {
        // move bullet
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    IEnumerator SafetyKill()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player")
        KillBullet();  
    }    
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag != "Player")
        KillBullet();  
    }

    private void KillBullet()
    {
        currentParticleEffect.transform.parent = null;
        currentParticleEffect.GetComponent<ParticleSystem>().enableEmission = false;
        Destroy(gameObject); // destroy ourselves
    }    
}
