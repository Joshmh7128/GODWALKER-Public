using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCanvasUIElement : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }

        if (canvasGroup.alpha < 1)
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }

    }
}
