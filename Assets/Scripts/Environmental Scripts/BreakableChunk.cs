using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChunk : MonoBehaviour
{
    [SerializeField] GameObject breakEffect; // the particle effect that plays when the chunk breaks
    [SerializeField] float hp, hpMax; // the HP and Max HP of our breakable chunk
    [SerializeField] Renderer ourRenderer; // the renderer of our chunk
    [SerializeField] float H, S, V; // color vars

    private void Start()
    {
        // get our color
        Color.RGBToHSV(ourRenderer.material.color, out H, out S, out V);
    }

    // breakable break function
    public void BreakableBreak()
    {
        hp--;
        ourRenderer.material.color = Color.HSVToRGB(H, S, hp/hpMax);
        if (hp <= 0)
        {
            Instantiate(breakEffect, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }
}
