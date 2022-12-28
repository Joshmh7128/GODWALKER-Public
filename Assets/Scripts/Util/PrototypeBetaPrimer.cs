using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeBetaPrimer : MonoBehaviour
{
    string targetScene;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();
        // request a new seed generation
        PlayerGenerationSeedManager.instance.BuildSeed();
        // load a scene based off of the generation manager's seed
        targetScene = PlayerGenerationSeedManager.instance.nextRoom.ToString();
        PlayerGenerationSeedManager.instance.currentPos++; // make sure to advance the current pos when moving to a new room
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);

    }

}
