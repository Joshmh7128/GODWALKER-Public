using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponQuickInfoHandler : MonoBehaviour
{
    // script manages the quick info windows 
    [SerializeField] TextMeshProUGUI weaponName, weaponDescription, hotnessText;
    [SerializeField] Slider hotnessSlider;
    [SerializeField] Image hotnessImage; // the image on the slider
    [SerializeField] CanvasGroup group, efGroup;
    [SerializeField] Color highColor, lowColor; // the high and lows of hotness
    float targetAlpha, efTargetAlpha; // what is the target alpha of our group
    float waitTime, efWaitTime; // how long we wait before fading out

    [SerializeField] GameObject warningObject;

    public bool shownHotnessTooltip; // have we shown the hotness tooltip?

    // setup our instance
    public static WeaponQuickInfoHandler instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // lerp our alpha
        group.alpha = Mathf.Lerp(group.alpha, targetAlpha, Time.fixedDeltaTime);

        // lower wait time
        if (waitTime >= 0)
            waitTime -= Time.fixedDeltaTime;

        if (waitTime <= 0)
            targetAlpha = 0;

        efGroup.alpha = Mathf.Lerp(efGroup.alpha, efTargetAlpha, Time.fixedDeltaTime);

        // lower wait time
        if (efWaitTime >= 0)
            efWaitTime -= Time.fixedDeltaTime;

        if (efWaitTime <= 0)
            efTargetAlpha = 0;

    }

    // this is called from the player weapon manager script
    public void InvokeInfo()
    {
        Invoke("ShowInfo", 0.1f);
    }

    // show our weapon info
    public void ShowInfo()
    {
        // show panel
        group.alpha = 1;
        // play the onboard animation
        gameObject.GetComponent<Animator>().Play("WeaponQuickInfoBlip");
        // update info
        weaponName.text = PlayerWeaponManager.instance.currentWeapon.weaponName;
        weaponDescription.text = PlayerWeaponManager.instance.currentWeapon.customInfo;
        float rageMulti = 2 * PlayerWeaponManager.instance.currentWeapon.rageMultiplier;
        hotnessText.text = "EFFECTIVENESS: " + rageMulti.ToString("0.00") + "X";
        hotnessSlider.value = PlayerWeaponManager.instance.currentWeapon.rageMultiplier;
        hotnessImage.color = Color.Lerp(lowColor, highColor, hotnessSlider.value);
        // set target alpha
        targetAlpha = 1;
        waitTime = 6;
    }

    public void EffectivenessLowered()
    {
        warningObject.SetActive(true);
        efGroup.alpha = 1; efTargetAlpha = 1; efWaitTime = 12;
        warningObject.GetComponent<Animator>().Play("WeaponQuickInfoBlip");
    }
}
