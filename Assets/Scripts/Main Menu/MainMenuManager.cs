using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    // our main menu UI pieces
    [SerializeField] Button playButton; // main play button
    [SerializeField] Button optionsButton; // options button 
    [SerializeField] Button exitButton; // exit game button
    [SerializeField] GameObject optionsPanel; // options panel where we store our options controls
    [SerializeField] Button toggleFullscreenButton; // toggle fullscreen
    [SerializeField] Slider masterVolumeSlider; // master slider
    [SerializeField] Slider effectsVolumeSlider; // effects slider
    [SerializeField] Slider musicVolumeSlider; // music slider
    [SerializeField] CanvasGroup fadeCanvas; // the image we are fading for the menu to game transition
    bool canFade; 
    float masterVolume; // master volume
    float effectsVolume; // effects volume
    float musicVolume; // music volume
    [SerializeField] AudioMixer masterMixer;

    // show and hide our options panel
    public void ToggleOptions()
    { optionsPanel.SetActive(!optionsPanel.activeInHierarchy); }

    // toggle fullscreen
    public void ToggleFullscreen()
    { Screen.fullScreen = !Screen.fullScreen; }

    // load in to our primer
    public void AdvanceToPrimer()
    {
        if (fadeCanvas.alpha < 1)
        {
            canFade = true;
        }

        if (fadeCanvas.alpha >= 0.99)
        {
            SceneManager.LoadScene("Primer", LoadSceneMode.Single);
        }
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        // make sure to set our volumes
        masterMixer.SetFloat("masterVolume", masterVolumeSlider.value);
        masterMixer.SetFloat("sfxVolume", effectsVolumeSlider.value);
        masterMixer.SetFloat("musicVolume", musicVolumeSlider.value);

        if (canFade == true)
        {
            fadeCanvas.alpha += 0.05f;
            AdvanceToPrimer();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;   
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
