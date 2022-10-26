using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadZoneHandler : MonoBehaviour
{
    /// script is used to build a load zone, in which assets are automatically turned off when they are inside it
    /// assets need to be inside a parent object, in this case a parent object is disabled
    /// script uses positions and checks the positions of the player to see if they are in it, instead of colliders
    /// 

    // our parameters
    [SerializeField] float zoneWidth, zoneHeight, zoneDepth; // our w, h, and depth of the zone
    [SerializeField] GameObject parentObject;

    PlayerController playerController;
    private void Start()
    {
        playerController = PlayerController.instance;
    }

    private void FixedUpdate()
    {
        parentObject.SetActive(HasPlayer());
    }

    // check to see if our player is within our bounding box
    bool HasPlayer()
    {
        if (playerController != null)
        {
            // check the X & Z -> check Y
            if (playerController.transform.position.x > transform.position.x - zoneWidth / 2 
                && playerController.transform.position.x < transform.position.x + zoneWidth / 2
                && playerController.transform.position.z > transform.position.z - zoneDepth / 2
                && playerController.transform.position.z < transform.position.z + zoneDepth / 2)
            {
                if (playerController.transform.position.y > transform.position.y - zoneHeight / 2 && playerController.transform.position.y < transform.position.y + zoneHeight / 2)
                    return true;
            }
        }
        return false;
    }

    // our gizmos
    private void OnDrawGizmos()
    {
        // draw bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(zoneWidth, zoneHeight, zoneDepth));
        // draw corners
        Gizmos.color = Color.cyan;

        float radius = (zoneWidth + zoneHeight + zoneDepth) * 0.005f;

        Gizmos.DrawSphere(transform.position + new Vector3(zoneWidth / 2, 0, 0), radius);
        Gizmos.DrawSphere(transform.position + new Vector3(-zoneWidth / 2, 0, 0), radius);

        Gizmos.DrawSphere(transform.position + new Vector3(0, zoneHeight/2, 0), radius);
        Gizmos.DrawSphere(transform.position + new Vector3(0, -zoneHeight/2, 0), radius);

        Gizmos.DrawSphere(transform.position + new Vector3(0, 0, zoneDepth / 2), radius);
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0, -zoneDepth / 2), radius);

        // draw center
        Gizmos.color = Color.blue;        
        if (HasPlayer()) Gizmos.color = Color.green; else Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, radius);


    }
}
