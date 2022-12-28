using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeBetaSceneTeleporter : MonoBehaviour
{
    [SerializeField] string prefix;
    [SerializeField] List<int> tpDestinations; // our teleporter destinations

    // used to send the player to a new scene
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Player")
        {
            Teleport();
        }
    }

    // the actual teleportation function
    void Teleport()
    {
        // get our target scene from the generation manager
        string targetScene = PlayerGenerationSeedManager.instance.nextRoom;
        PlayerGenerationSeedManager.instance.currentPos++;
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
