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
    [SerializeField] GameObject mainParent, optionsParent, creditsParent, realThanks, funnyThanks;
    [SerializeField] TMP_InputField senseInput;
    [SerializeField] Toggle quickStartToggle, heightScaleToggle; // do we quick start?

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

        if (PlayerPrefs.GetString("QuickStart", "off") == "on")
            quickStartToggle.isOn = true;

        if (PlayerPrefs.GetString("ShouldWidthScale", "false") == "true")
            heightScaleToggle.isOn = true;
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
        if (!quickStartToggle.isOn)
        {
            SceneManager.LoadScene("TakeTheGodheart");
            PlayerPrefs.SetString("QuickStart", "off");
        }

        if (quickStartToggle.isOn)
        {
            SceneManager.LoadScene("Game Start");
            PlayerPrefs.SetString("QuickStart", "on");
        }

        if (PlayerPrefs.GetString("QuickStart", "off") == "off")
            quickStartToggle.isOn = false;

        if (PlayerPrefs.GetString("QuickStart", "off") == "on")
            quickStartToggle.isOn = true;
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

    // toggle the credits panel
    public void ToggleCredits()
    {
        mainParent.SetActive(!mainParent.activeInHierarchy);
        creditsParent.SetActive(!creditsParent.activeInHierarchy);
    }

    // toggle the funny thanks
    public void ToggleThanks()
    {
        realThanks.SetActive(!realThanks.activeInHierarchy);    
        funnyThanks.SetActive(!funnyThanks.activeInHierarchy);
    }

    // toggle our screensize settings
    public void ToggleScreenScaling()
    {
        if (heightScaleToggle.isOn)
            PlayerPrefs.SetString("ShouldWidthScale", "true");

        if (!heightScaleToggle.isOn)
            PlayerPrefs.SetString("ShouldWidthScale", "false");

    }
}
