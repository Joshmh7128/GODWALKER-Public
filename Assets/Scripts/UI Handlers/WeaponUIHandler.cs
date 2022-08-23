using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIHandler : MonoBehaviour
{
    // this script serves as a handler for dynamic weapon UI. it uses weapon class data to display UI information

    WeaponClass weaponClass; // the weaponclass of our weapon

    // the set of UI elements that we can use to represent our weapon
    [SerializeField] Slider ammoSlider, reloadSlider; // our weapon's slider
    [SerializeField] CanvasGroup reloadSliderGroup; // the reload slider group
    [SerializeField] RectTransform reticleRight, reticleLeft, reticleTop, reticleBottom; // all four of our reticle lines 
    Vector3 reticleLeftOrigin, reticleRightOrigin, reticleTopOrigin, reticleBottomOrigin; // the four origin points of our reticles
    [SerializeField] float reticleSpreadResponseMagnitude = 100f; // how much our reticles represent the spread
    [SerializeField] Text ammoText; // displays the ammo in numerical form for us

    // run on start to setup our weapon
    void SetupUI()
    {
        // get our weaponClass
        weaponClass = GetComponent<WeaponClass>();
        // setup our reticle points
        reticleLeftOrigin = reticleLeft.anchoredPosition;
        reticleRightOrigin = reticleRight.anchoredPosition;
        reticleTopOrigin = reticleTop.anchoredPosition;
        reticleBottomOrigin = reticleBottom.anchoredPosition;
    }

    // run the UI
    void ProcessUI()
    {
        // ammo slider
        if (ammoSlider != null)
        {
            // set the values of the slider
            ammoSlider.maxValue = weaponClass.maxMagazine;
            ammoSlider.value = weaponClass.currentMagazine;
        }

        // dynamic reticle lerping
        reticleRight.anchoredPosition = Vector2.Lerp(reticleRight.anchoredPosition, reticleRightOrigin, 2f * Time.deltaTime);
        reticleLeft.anchoredPosition = Vector2.Lerp(reticleLeft.anchoredPosition, reticleLeftOrigin, 2f * Time.deltaTime);
        reticleTop.anchoredPosition = Vector2.Lerp(reticleTop.anchoredPosition, reticleTopOrigin, 2f * Time.deltaTime);
        reticleBottom.anchoredPosition = Vector2.Lerp(reticleBottom.anchoredPosition, reticleBottomOrigin, 2f * Time.deltaTime);

        // ammo text
        if (ammoText != null)
        {
            ammoText.text = weaponClass.currentMagazine + " / " + weaponClass.maxMagazine;
        }
    }

    // kick ui, call when the weapon fires
    public void KickUI()
    {
        // movement outwards
        reticleRight.anchoredPosition = new Vector2(reticleRightOrigin.x + weaponClass.spreadX * reticleSpreadResponseMagnitude, 0);
        reticleLeft.anchoredPosition = new Vector2(reticleLeftOrigin.x + -weaponClass.spreadX * reticleSpreadResponseMagnitude, 0);
        reticleTop.anchoredPosition = new Vector2(0, reticleTopOrigin.y + weaponClass.spreadY * reticleSpreadResponseMagnitude);
        reticleBottom.anchoredPosition = new Vector2(0, reticleBottomOrigin.y + -weaponClass.spreadY * reticleSpreadResponseMagnitude);
    }

    public void TriggerReload(float reloadTime)
    {
        reloadSlider.maxValue = reloadTime;
        reloadSlider.value = 0;
    }

    void ProcessReload()
    {
        // count up
        if (reloadSlider.value < reloadSlider.maxValue)
        {
            reloadSliderGroup.alpha = 1f;
            reloadSlider.value += Time.deltaTime;
        }

        // when we hit
        if (reloadSlider.value == reloadSlider.maxValue)
        {
            if (reloadSliderGroup.alpha > 0)
            {
                reloadSliderGroup.alpha -= 0.1f;
            }
        }
    }

    // cancel our reload
    public void CancelReload()
    {
        Debug.Log("Cancel Reload called on weapon ui handler");
        reloadSliderGroup.alpha = 0f;
        reloadSlider.value = reloadSlider.maxValue;
    }

    // when this weapon is dropped
    public void OnDrop()
    {
        reticleLeft.anchoredPosition = reticleLeftOrigin;
        reticleRight.anchoredPosition = reticleRightOrigin;
        reticleTop.anchoredPosition = reticleTopOrigin;
        reticleBottom.anchoredPosition = reticleBottomOrigin;
    }

    private void Start()
    {
        SetupUI();
    }

    private void Update()
    {
        // process our UI
        ProcessUI();
        // process the reload
        ProcessReload();
    }

}
