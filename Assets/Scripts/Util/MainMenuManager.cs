using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI aimSensitivityText, volumeText, musicVolumeText;
    [SerializeField] Slider aimSenseSlider, volumeSlider, musicVolumeSlider;
    [SerializeField] GameObject mainParent, optionsParent, creditsParent, realThanks, funnyThanks;
    [SerializeField] TMP_InputField senseInput;
    [SerializeField] Toggle quickStartToggle, heightScaleToggle, dataToggle; // do we quick start?
    [SerializeField] AudioMixer audioMixer; // our audio mixer

    float aimSensitivity, volume, musicVolume;

    private void Start()
    {
        // set sensitivity
        aimSensitivity = PlayerPrefs.GetFloat("sensitivity", 15); 
        aimSenseSlider.value = aimSensitivity;
        senseInput.text = aimSensitivity.ToString();

        // set volume
        volume = PlayerPrefs.GetFloat("masterVolume", 50);
        volumeSlider.value = volume;

        musicVolume = PlayerPrefs.GetFloat("musicVolume", 50);
        musicVolumeSlider.value = musicVolume;

        musicVolumeText.text = "Music Level: " + musicVolume.ToString();
        volumeText.text = "Volume Level: " + volume.ToString();

        if (PlayerPrefs.GetString("QuickStart", "off") == "on")
            quickStartToggle.isOn = true;

        if (PlayerPrefs.GetString("ShouldWidthScale", "false") == "true")
            heightScaleToggle.isOn = true;

        if (PlayerPrefs.GetString("CollectData", "true") == "true")
            dataToggle.isOn = true;

        if (PlayerPrefs.GetString("CollectData", "true") == "false")
            dataToggle.isOn = false;

        // setup our unique UID
        if (PlayerPrefs.GetString("UID", "") == "")
        {
            PlayerPrefs.SetString("UID", RandomString(32));
        }
    }

    string RandomString(int length)
    {
        string uid = "user";

        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        for (int i = 0; i < length; i++)
            uid += chars[Random.Range(0, chars.Length)];

        // if we are in the editor, name it josh so that we can filter it out
        if (Application.isEditor)
            uid = "JOSH";

        return uid;
    }

    public void SetSensitivity()
    {
        aimSensitivity = Mathf.Round(aimSenseSlider.value * 10) * 0.1f;

        senseInput.text = (Mathf.Round(aimSenseSlider.value * 10) * 0.1f).ToString();
        aimSensitivityText.text = "Sensitivity Slider. Current Sensitivity = " + aimSensitivity;

        PlayerPrefs.SetFloat("sensitivity", aimSensitivity);
        Debug.Log(aimSensitivity);
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

    // toggle our data collection
    public void ToggleDataCollection()
    {
        if (dataToggle.isOn)
            PlayerPrefs.SetString("CollectData", "true");

        if (!dataToggle.isOn)
            PlayerPrefs.SetString("CollectData", "false");
    }

    // toggle our screensize settings
    public void ToggleScreenScaling()
    {
        if (heightScaleToggle.isOn)
            PlayerPrefs.SetString("ShouldWidthScale", "true");

        if (!heightScaleToggle.isOn)
            PlayerPrefs.SetString("ShouldWidthScale", "false");
    }

    public void SetAudio()
    {
        volume = volumeSlider.value;
        volumeText.text = "Volume Level: " + volume.ToString();
        PlayerPrefs.SetFloat("masterVolume", volume);
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume / 100) * 20);
        
        // then music volume
        musicVolume = musicVolumeSlider.value;
        musicVolumeText.text = "Music Volume Level: " + musicVolume.ToString();
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVolume / 100) * 20);
        
    }
}
