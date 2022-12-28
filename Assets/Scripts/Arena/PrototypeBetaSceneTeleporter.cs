using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeBetaSceneTeleporter : MonoBehaviour
{
    // used to send the player to a new scene
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Player")
        {

        }
    }

    // the actual teleportation function
    void Teleport()
    {

    }
}
