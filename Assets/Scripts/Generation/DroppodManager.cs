using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class DroppodManager : MonoBehaviour
{
    bool canLaunch = false;
    bool isFlying = false;
    [SerializeField] Renderer greenZoneRenderer;
    [SerializeField] Material dimGreen;
    [SerializeField] Material brightGreen;
    [SerializeField] GenerationManager generationManager;
    [SerializeField] Transform playerTrans;
    [SerializeField] Vector3 targetPosStart;
    [SerializeField] Vector3 targetPosFin;
    [SerializeField] Vector3 movementDirection;
    [SerializeField] MovingPlatform ourPlatform;

    private void Update()
    {
        if (canLaunch == true)
        {
            // make our green zone green
            greenZoneRenderer.material = brightGreen;
            // launch
            if (ReInput.players.GetPlayer(0).GetButtonDown("SpacePress"))
            {
                isFlying = true;
                generationManager.MapRegen();
                // launch the drop pod
                // gameObject.GetComponent<Animator>().Play("Asteroid Hop");
                StartCoroutine("LaunchPod");
            }
        }
        else
        {
            greenZoneRenderer.material = dimGreen;
        }

        if (isFlying == true)
        {
            //playerTrans.SetParent(transform);
        }
    }

    IEnumerator LaunchPod()
    {
        ourPlatform.targetPos = targetPosFin;
        yield return new WaitForSeconds(10f);
        ourPlatform.targetPos = targetPosStart;
    }

    // when the player enters the green zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            // playerTrans.SetParent(transform);
            canLaunch = true;
    }    

    // when the player enters the green zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
            // playerTrans.SetParent(null);
            canLaunch = false;
    }
}
