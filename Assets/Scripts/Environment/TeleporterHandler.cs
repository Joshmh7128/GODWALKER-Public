using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterHandler : MonoBehaviour
{
    // script exists to teleport player from room to room
    public string nextRoom; // the room we are going to next
    [SerializeField] GameObject visuals; // the parent object of all our teleporter visuals

    private void Start()
    {
        // make sure we dont destroy this script on load
        DontDestroyOnLoad(this);

        // make sure our teleporter is off
        visuals.SetActive(false);

    }

    // call this when we activate the teleporter
    public void ActivateTeleporter()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // load the next scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextRoom);
            // start our coroutine
            StartCoroutine(SceneCheck());

        }
    }

    // teleports the player to 0,0,0 and makes sure we don't destroy until we are in the next scene
    IEnumerator SceneCheck()
    {
        // wait until the scene loads
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == nextRoom);
        // teleport the player
        PlayerController.instance.Teleport(Vector3.zero);
        // destroy ourselves
        Destroy(gameObject);
    }

}
