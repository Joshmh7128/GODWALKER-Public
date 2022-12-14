using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_LineRenderTarget : EnemyBehaviour
{
    // used to manage a line renderer to target the player. lerps between two colors overtime
    // has ability to use charge particle as well

    [SerializeField] LineRenderer lineRenderer;
    Vector3 targetPos; // where is this line renderer going?
    [SerializeField] Color startColor, endColor; // our start and end colors
    bool running = false; // we aren't running yet


    private void FixedUpdate()
    {
        if (running)
        {
            // set our line renderer positions

        }
    }

    public override IEnumerator MainCoroutine()
    {
        // set our second position of the line renderer
        running = true; // we have started running

        yield return null; 
    }
}
