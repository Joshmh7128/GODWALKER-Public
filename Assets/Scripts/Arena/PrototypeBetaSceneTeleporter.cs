using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeBetaSceneTeleporter : MonoBehaviour
{
    [SerializeField] string prefix;
    [SerializeField] List<int> tpDestinations; // our teleporter destinations
    bool used = false; // has this been used yet?
    [SerializeField] string targetScene;
    private void Start()
    {
    }

    // used to send the player to a new scene
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Player")
        {
            if (!used)
            {
                Teleport();
            }
        }
    }

    // the actual teleportation function
    void Teleport()
    {
        StartCoroutine(TeleportBuffer());
    }

    // run a coroutine because we are getting teleportations multiple times per trigger collide
    IEnumerator TeleportBuffer()
    {
        yield return new WaitForSeconds(Random.Range(0.01f, 0.06f));
        if (used) yield return null;
        used = true;
        Debug.Log("teleporting");
        // get our target scene from the generation manager
        targetScene = PlayerGenerationSeedManager.instance.nextRoom.ToString();
        PlayerGenerationSeedManager.instance.currentCombatPos++;
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

}
