using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAnimation : MonoBehaviour
{
    [SerializeField] float fps; // what is the framerate of the animation?
    [SerializeField] List<Sprite> sprites; // frames of the animation
    int i; // our index

    private void Start()
    {
        StartCoroutine(DoAnimation());
    }

    IEnumerator DoAnimation()
    {
        yield return new WaitForSecondsRealtime(fps / 60);
        i++;
        if (i > sprites.Count) i = 0;
        gameObject.GetComponent<LineRenderer>().material.mainTexture = sprites[i].texture;
    }
}
