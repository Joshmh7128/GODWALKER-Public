using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChunk : MonoBehaviour
{
    [SerializeField] GameObject breakEffect; // the particle effect that plays when the chunk breaks
    [SerializeField] float hp, hpMax; // the HP and Max HP of our breakable chunk
    [SerializeField] Renderer ourRenderer; // the renderer of our chunk
    [SerializeField] float H, S, V; // color vars
    [SerializeField] GameObject smallChunk; // the same chunk but smaller that the player can pick up. large chunk breaks = small chunks fly out

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
            // choose a random amount of small chunks then spawn them in
            for (float i = 0; i < 10; i++)
            {
                Instantiate(smallChunk, transform.position + new Vector3(Random.Range(-3,3), 0, Random.Range(-3, 3)), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            }

            Instantiate(breakEffect, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            Destroy(gameObject);
        }
    }


}
