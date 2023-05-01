using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodheartSceneManager : MonoBehaviour
{
    // reset prefab
    [SerializeField] GameObject autoResetPrefab;
    [SerializeField] CanvasGroup blackFade, whiteFade;

    bool touched; 

    private void FixedUpdate()
    {
        if (blackFade.alpha > 0)
        {
            blackFade.alpha -= Time.fixedDeltaTime;
        }

        if (touched)
        {
            whiteFade.alpha += Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            touched = true;
            StartCoroutine(LoadForward());
        }


    }

    IEnumerator LoadForward()
    {
        yield return new WaitForSecondsRealtime(1f);
        Instantiate(autoResetPrefab);
    }
}
