using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChunk : MonoBehaviour
{
    [SerializeField] GameObject breakEffect; // the particle effect that plays when the chunk breaks
    [SerializeField] float hp, hpMax; // the HP and Max HP of our breakable chunk
    [SerializeField] Renderer ourRenderer; // the renderer of our chunk
    [SerializeField] float H, S, V; // color vars
    [SerializeField] float dropAmount; // color vars
    [SerializeField] GameObject smallChunk; // the same chunk but smaller that the player can pick up. large chunk breaks = small chunks fly out
    Color ourColor;

    private void Start()
    {
        // get our color
        ourColor = ourRenderer.material.GetColor("_EmissionColor"); ;
    }

    // breakable break function
    public void BreakableBreak()
    {
        // subtract our HP
        hp--;
        // change our color
        ourColor = Color.HSVToRGB(H, S, (hp/hpMax)*0.25f);

        // set our HP based on the amount we have out of our maximum
        ourRenderer.material.SetColor("_EmissionColor", ourColor);

        if (hp <= 0)
        {
            // choose a random amount of small chunks then spawn them in
            for (float i = 0; i < dropAmount; i++)
            {
                Vector3 spawnpos = transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
                Instantiate(smallChunk, spawnpos, Quaternion.Euler(new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3))));
            }

            Instantiate(breakEffect, transform.position, Quaternion.identity, null);
            Destroy(gameObject);
        }
    }


}
