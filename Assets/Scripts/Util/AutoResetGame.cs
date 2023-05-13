using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoResetGame : MonoBehaviour
{
    // load our primer scene
    //void Start() => StartCoroutine(Buffer());

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(Buffer());
        }
    }

    public string RestartScene; // which scene do we restart from?

    IEnumerator Buffer()
    {
        PlayerGenerationSeedManager.instance.ResetRun();

        // find and kill all players
        try
        {
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                Destroy(player.gameObject);
            }
        }

        catch
        {
            Debug.Log("No Players to destroy!");
        }

        finally
        {
            // Debug.LogAssertion("Reloading Scene...");
        }

        yield return new WaitForSeconds(0f);
         
        // kill all tween loading rooms
        KillAllTweens();

        // check player pref
        if (PlayerPrefs.GetString("QuickStart", "off") == "on" && RestartScene != "PlayerMainMenu")
            RestartScene = "Game Start";

        // reload
        SceneManager.LoadScene(RestartScene);
    }

    void KillAllTweens()
    {
        List<TweenRoomHandler> tweens = FindObjectsOfType<TweenRoomHandler>().ToList();
        foreach(TweenRoomHandler tween in tweens)
        {
            Destroy(tween.gameObject);
        }
    }
}
