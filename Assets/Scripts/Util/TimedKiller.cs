using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedKiller : MonoBehaviour
{
    /// <summary>
    /// use this script to kill an object after X seconds of existing
    /// </summary>

    [SerializeField] float killTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(killTime);
        Destroy(gameObject);
    }
}
