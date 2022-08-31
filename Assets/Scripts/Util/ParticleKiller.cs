using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour
{
    // the particle system we're killing
    [SerializeField] ParticleSystem pSys;
    private void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(pSys.main.duration);
        Destroy(gameObject);
    }
}
