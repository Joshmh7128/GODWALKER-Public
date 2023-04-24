using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class PlayerRageManager : MonoBehaviour
{
    // create instance
    public static PlayerRageManager instance;
    private void Awake() => instance = this;

    // agnostic variables
    public Slider rageSlider; // our slider
    public Slider rageLerp; // our lerp slider
    public float rageLerpSpeed; // how fast our lerper goes
    public Image sliderImage; // our slider image on our lerper
    public Image rageVignette; // our rage vignette
    public GameObject flameVFX, screenParticles; // visual effects for our rage mode
    public TextMeshProUGUI reachedGODWALKERDisplay; // has this player reached godwalker?

    #region // v2 variables

    [Header("V2 elements")]
    // we want to generate enough rage and then go into godwalker for 10 seconds
    public float rageAmount; // our v2 rage amount
    public float maxRage; // what is the maximum rage we can have?
    public float reductionDelta, originalReductionDelta, godwalkerReductionDelta, godwalkerReductionDeltaAdditional, originalAdditional; // our rage reduction delta
    public Color startColor, endColor, godwalkingColor; // our start and end colors
    public float maxSpeedBoost, currentSpeedBoost; // how much faster do we move BEFORE entering Godwalker?
    public float godwalkerSpeedBoost; // how fast do we move in Godwalker?
    public float godwalkerTime; // how long do we remain in godwalker?
    public bool godwalking; // oh, it's happening, baby. 
    public GameObject godwalkerVolume; // the godwalker VFX volume

    public TextMeshProUGUI rageLevelDisplay;
    #endregion

    [Header("V3 elements")]
    public Slider backSlider; // our slider representing how far away we are from the next level
    public List<Color> rageColors;
    public List<float> ammoRechargeMods; // how quickly ammo refills
    
    // setup rage levels
    public enum RageLevels
    {
        WALKER,
        DANCER,
        TRICKER,
        SMACKER,
        KILLER
    }

    public RageLevels rageLevel; // which rage level we're currently on

    [SerializeField] float overRage; // rage that exceeds the godwalker meter
    [SerializeField] List<float> overRageGates = new List<float>(); // how much overrage we have to build to get to the next level, each starts from 0

    private void Start()
    {
        originalReductionDelta = godwalkerReductionDelta; // set the original delta
        originalAdditional = godwalkerReductionDeltaAdditional;

    }

    // public function to add rage
    public void AddRage(float amount)
    {
        if (rageAmount + amount <= maxRage)
        {
            rageAmount += amount;
            overRage += amount;
            
        } else { rageAmount = maxRage; }

        rageAmount = Mathf.Clamp(rageAmount, 0, maxRage);
    }

    private void FixedUpdate()
    {
        ProcessRage();
    }

    // process our new rage
    public void ProcessRage()
    {
        // reduce our rage if we have not maxed out the meter
        if (rageAmount != maxRage && rageAmount > 0 && !godwalking)
        {
            rageAmount -= reductionDelta * Time.fixedDeltaTime;
            overRage -= reductionDelta * Time.fixedDeltaTime;
        }

        // process our overRage
        if (overRage > overRageGates[(int)rageLevel])
        {
            rageLevel++;
            overRage = 0; // reset it
        }

        // process our UI 
        ProcessUI();

        // process inputs
        ProcessInput();

        // process godwalker
        ProcessGodwalking();

    }

    void ProcessGodwalking()
    {
        if (godwalking)
        {
            // effects
            godwalkerVolume.SetActive(true);
            flameVFX.SetActive(true);
            // go apeshit
            rageLevelDisplay.text = "GOD" + rageLevel.ToString();
            // set rage vignette to godwalker color
            rageVignette.color = godwalkingColor;
            sliderImage.color = godwalkingColor;
            // reduce our rage by the amount we are spending
            rageAmount -= (godwalkerReductionDelta + godwalkerReductionDeltaAdditional) * Time.fixedDeltaTime;
            overRage -= (godwalkerReductionDelta + godwalkerReductionDeltaAdditional) * Time.fixedDeltaTime;
            // refill HP
            PlayerStatManager.instance.AddHealth(20 * Time.fixedDeltaTime);
            // if we're godwalking raise our speedboost
            currentSpeedBoost = godwalkerSpeedBoost;
            godwalkerReductionDelta += godwalkerReductionDeltaAdditional * Time.fixedDeltaTime;

            // refill all our weapon ammo 
            foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
            {
                weapon.GetComponent<WeaponClass>().currentMagazine = weapon.GetComponent<WeaponClass>().maxMagazine;
            }

            // when do we end godwalking?
            if (rageAmount <= 0)
            {
                // reset the delta & additional
                godwalkerReductionDelta = originalReductionDelta; // reset
                godwalkerReductionDeltaAdditional = originalAdditional;
                // deactivate the fx
                flameVFX.SetActive(false);
                // we are no longer godwalking
                godwalking = false;
                godwalkerVolume.SetActive(false);
                rageLevelDisplay.text = "";
            }

        }

        if (!godwalking)
        {
            // set our speed boost
            currentSpeedBoost = 1 + maxSpeedBoost * rageAmount / maxRage;
        }
    }

    void ProcessInput()
    {

        // entering godwalker - if our bar is full and we press G
        if (Input.GetKeyDown(KeyCode.G) && rageAmount == maxRage)
        {
            if (!godwalking)
                // kick feel
                PlayerGodfeelManager.instance.KickFeel();

            // we are now godwalking
            godwalking = true;

            // check to see if we increase our reduction delta
            StartCoroutine(ReductionDeltaIncreaseCheck());
        }

    }

    void ProcessUI()
    {   
        // run our UI
        rageSlider.value = rageAmount / maxRage;

        // setup lerp slider
        rageLerp.minValue = rageSlider.minValue;
        rageLerp.maxValue = rageSlider.maxValue;
        rageLerp.value = Mathf.Lerp(rageLerp.value, rageSlider.value, rageLerpSpeed * Time.fixedDeltaTime);

        // run our back UI
        backSlider.minValue = 0;
        backSlider.maxValue = overRageGates[(int)rageLevel];
        backSlider.value = overRage / overRageGates[(int)rageLevel];

        // colors while godwalking
        sliderImage.color = Color.Lerp(startColor, endColor, rageSlider.value);
        rageVignette.color = Color.Lerp(startColor, endColor, rageSlider.value);

        // lerp the color of our images
        if (!godwalking)
        {
            sliderImage.color = Color.Lerp(startColor, endColor, rageSlider.value);
            rageVignette.color = Color.Lerp(startColor, endColor, rageSlider.value);
        }

        // if we're at our max rage and we're not godwalking
        if (rageAmount == maxRage && !godwalking)
        {
            rageLevelDisplay.text = "GODWALKER READY PRESS G TO ACTIVATE";
        }
    }

    // add to the delta
    IEnumerator ReductionDeltaIncreaseCheck()
    {
        // if the player maintains godwalker for 20 seconds
        yield return new WaitForSecondsRealtime(20f);
        // increase the rage by 10%
        if (godwalking)
            originalAdditional += 2.5f * 0.1f;
    }

}
