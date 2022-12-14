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
    [SerializeField] Transform alignmentTransform; // the transform we base our forward off of
    float timePassed;
    [SerializeField] float startSize, endSize; // the start and end size of our line
    private void FixedUpdate()
    {
        if (running)
        {
            // continue time passed
            timePassed += Time.fixedDeltaTime;
            // set our line renderer positions
            targetPos = alignmentTransform.forward * 100f;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetPos);
            // lerp our line renderer colors
            Color lerpColor = Color.Lerp(startColor, endColor, timePassed / behaviourTime);
            lineRenderer.startColor = lerpColor;
            lineRenderer.endColor = lerpColor;

        }
    }

    public override IEnumerator MainCoroutine()
    {
        // set our second position of the line renderer
        running = true; // we have started running

        yield return new WaitForSeconds(behaviourTime);
        complete = true;
        yield return null; 
    }
}
