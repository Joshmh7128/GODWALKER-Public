using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] float scrollX, scrollY, offsetX, offsetY;
    Renderer objRend;

    private void Start()
    {
        objRend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // normal scrolling
        offsetX += Time.fixedDeltaTime * scrollX;
        offsetY += Time.fixedDeltaTime * scrollY;
        // set
        objRend.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
