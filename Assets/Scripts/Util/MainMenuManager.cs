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
    [SerializeField] GameObject mainParent, optionsParent;
    [SerializeField] TMP_InputField senseInput;

    float aimSensitivity;

    private void Start()
    {
        PlayerPrefs.GetFloat("sensitivity", aimSensitivity);
        // set our sensitivity slider to the last used sense
        if (aimSensitivity != 0)
        {
            aimSenseSlider.value = aimSensitivity;
            senseInput.text = aimSensitivity.ToString();
        }
        else
        {
            aimSensitivity = 15;
            aimSenseSlider.value = aimSensitivity;
            senseInput.text = aimSensitivity.ToString();
        }
    }

    private void Update()
    {

    }

    public void SetSensitivity()
    {
        PlayerPrefs.GetFloat("sensitivity", aimSensitivity);
        aimSensitivity = Mathf.Round(aimSenseSlider.value * 10) * 0.1f;

        PlayerPrefs.SetFloat("sensitivity", aimSensitivity);

        senseInput.text = (Mathf.Round(aimSenseSlider.value * 10) * 0.1f).ToString();
        aimSensitivityText.text = "Sensitivity Slider. Current Sensitivity = " + aimSensitivity;
    }   
    
    public void SetSensitivityText()
    {
        PlayerPrefs.GetFloat("sensitivity", aimSensitivity);
        // aimSensitivityText.text = "Sensitivity Slider. Current Sensitivity = " + aimSensitivity;

        aimSensitivity = Mathf.Round(float.Parse(senseInput.text) * 10) * 0.1f;

        aimSenseSlider.value = aimSensitivity;
        senseInput.text = aimSensitivity.ToString("F2");
        PlayerPrefs.SetFloat("sensitivity", aimSensitivity);

        senseInput.text = aimSensitivity.ToString();
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
    
    // toggle the options panel
    public void ToggleOptions()
    {
        mainParent.SetActive(!mainParent.activeInHierarchy);
        optionsParent.SetActive(!optionsParent.activeInHierarchy);
    }
}
