using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour
{
    // the particle system we're killing
    [SerializeField] ParticleSystem pSys;
    private void Start()
    {
        if (pSys == null)
        { pSys = GetComponent<ParticleSystem>(); }
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(pSys.main.duration);
        Destroy(gameObject);
    }
}
