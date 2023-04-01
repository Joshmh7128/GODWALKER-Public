using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // load the game
    public void LoadGame()
    {
        SceneManager.LoadScene("Game Start");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
