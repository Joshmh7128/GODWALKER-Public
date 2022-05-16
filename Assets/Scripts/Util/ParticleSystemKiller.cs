using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemKiller : MonoBehaviour
{
    protected ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}

