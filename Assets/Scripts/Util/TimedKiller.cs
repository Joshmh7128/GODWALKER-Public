using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedKiller : MonoBehaviour
{
    /// <summary>
    /// use this script to kill an object after X seconds of existing
    /// </summary>

    [SerializeField] float killTime;
    [SerializeField] bool onlyParentless, disable; // only kill when parentless?
    bool used;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!onlyParentless)
            StartCoroutine(Countdown());
    }

    private void FixedUpdate()
    {
        if (onlyParentless && transform.parent == null && !used)
            StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        used = true;
        yield return new WaitForSeconds(killTime);

        if (!disable)
            Destroy(gameObject);
        if (disable)
            gameObject.SetActive(false);
    }
}
