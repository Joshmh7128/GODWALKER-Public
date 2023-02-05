using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoResetGame : MonoBehaviour
{
    // load our primer scene
    void Start() => StartCoroutine(Buffer());

    public string RestartScene; // which scene do we restart from?

    IEnumerator Buffer()
    {
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
            Debug.LogAssertion("Reloading Scene...");
        }

        yield return new WaitForSeconds(2f);
        // reload
        SceneManager.LoadScene(RestartScene);
    }
}
