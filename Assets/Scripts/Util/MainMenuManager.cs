using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI aimSensitivityText;
    [SerializeField] Slider aimSenseSlider; 

    float aimSensitivity;

    private void Start()
    {
        PlayerPrefs.GetFloat("sensitivity", aimSensitivity);
        // set our sensitivity slider to the last used sense
        if (aimSensitivity != 0)
        {
            aimSenseSlider.value = aimSensitivity;
        }
        else
        {
            aimSensitivity = 15;
            aimSenseSlider.value = aimSensitivity;
        }
    }

    private void Update()
    {
        SetSensitivity();
    }

    public void SetSensitivity()
    {
        PlayerPrefs.GetFloat("sensitivity", aimSensitivity);
        aimSensitivityText.text = "Sensitivity Slider. Current Sensitivity = " + aimSensitivity;
        aimSensitivity = aimSenseSlider.value;

        PlayerPrefs.SetFloat("sensitivity", aimSensitivity);

    }

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
