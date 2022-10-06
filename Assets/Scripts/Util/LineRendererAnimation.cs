using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAnimation : MonoBehaviour
{
    [SerializeField] float fps; // what is the framerate of the animation?
    [SerializeField] List<Sprite> sprites; // frames of the animation
    int i; // our index
    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine(DoAnimation());
    }

    IEnumerator DoAnimation()
    {
        float x = 1 / (60 / fps);
        yield return new WaitForSecondsRealtime(x);
        i++;
        if (i >= sprites.Count) i = 0;
        lineRenderer.material.mainTexture = sprites[i].texture;
        StartCoroutine(DoAnimation());
    }
}
