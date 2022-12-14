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
    [SerializeField] Transform alignmentTransform, projectileOrigin, particleFX; // the transform we base our forward off of
    float timePassed;
    [SerializeField] float startSize, endSize; // the start and end size of our line
    [SerializeField] float particleStartSize, particleEndSize; // the start and end size of our line
    private void FixedUpdate()
    {
        if (running)
        {
            // continue time passed
            timePassed += Time.fixedDeltaTime;
            // set our line renderer positions
            targetPos = alignmentTransform.position + alignmentTransform.forward * 100f;
            if (lineRenderer)
            {
                // linecast to target pos
                RaycastHit hit; Physics.Linecast(projectileOrigin.position, targetPos, out hit);
                lineRenderer.SetPosition(0, alignmentTransform.position);
                lineRenderer.SetPosition(1, hit.point);
                // lerp our line renderer colors
                Color lerpColor = Color.Lerp(startColor, endColor, timePassed / behaviourTime);
                lineRenderer.startColor = lerpColor;
                lineRenderer.endColor = lerpColor;
                // set line size
                float lerpSize = Mathf.Lerp(startSize, endSize, timePassed / behaviourTime);
                lineRenderer.startWidth = lerpSize;
                lineRenderer.endWidth = lerpSize;
            }
            // set particle size
            if (particleFX)
            {
                Vector3 partSize = new Vector3();
                partSize.x = Mathf.Lerp(particleStartSize, particleEndSize, timePassed / behaviourTime);
                partSize.y = Mathf.Lerp(particleStartSize, particleEndSize, timePassed / behaviourTime);
                partSize.z = Mathf.Lerp(particleStartSize, particleEndSize, timePassed / behaviourTime);
                particleFX.localScale = partSize;
            }
        }

        if (complete)
        {
            try
            {
                lineRenderer.SetPosition(0, new Vector3(9999, 9999, 9999));
                lineRenderer.SetPosition(1, new Vector3(9999, 9999, 9999));
            }
            catch { }
            try
            {
                particleFX.localScale = Vector3.zero;
            } catch { }
        }
    }

    public override IEnumerator MainCoroutine()
    {
        timePassed = 0;
        // set our second position of the line renderer
        running = true; // we have started running

        yield return new WaitForSeconds(behaviourTime);
        running = false; particleFX.localScale = Vector3.zero;
        complete = true;
        yield return null; 
    }
}
