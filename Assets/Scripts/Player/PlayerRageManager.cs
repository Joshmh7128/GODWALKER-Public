using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRageManager : MonoBehaviour
{
    // create instance
    public static PlayerRageManager instance;
    private void Awake() => instance = this;
   
    public enum Behaviours
    {
        original,
        v2
    }

    public Behaviours behaviour; // what is our behaviour?

    #region // original variables
    // setup rage levels
    public enum RageLevels
    {
        benign, 
        wrecker,
        demonic,
        eviscerator,
        godwalker
    }

    public RageLevels rageLevel; // which rage level we're currently on
    public float originalRageAmount; // our total amount of rage

    // UI variables
    public List<Color> rageColors;
    public List<float> levelGates; // where each level of rage ends
    public List<string> levelNames; // the names of each level
    public string levelName; // the name of the current level to display
    public TextMeshProUGUI rageLevelDisplay;
    public List<float> levelDeltas; // how quickly each level reduces
    public List<float> levelMods; // how quickly each level reduces
    public List<float> ammoRechargeMods; // how quickly ammo refills
    public float levelDelta; // our current level delta

   
    public List<float> levelHPRegen; // how much HP we regen every second per level

    // effect variables
    public List<float> movementMultipliers;
    public List<float> damageMultipliers;
    #endregion

    // agnostic variables
    public Slider rageSlider; // our slider
    public Slider rageLerp; // our lerp slider
    public float rageLerpSpeed; // how fast our lerper goes
    public Image sliderImage; // our slider image on our lerper
    public Image rageVignette; // our rage vignette
    public GameObject flameVFX, screenParticles; // visual effects for our rage mode
    public TextMeshProUGUI reachedGODWALKERDisplay; // has this player reached godwalker?
    float GodwalkerTime; // how long we've been in godwalker


    #region // v2 variables

    [Header("V2 elements")]
    // we want to generate enough rage and then go into godwalker for 10 seconds
    public float rageAmount; // our v2 rage amount
    public float maxRage; // what is the maximum rage we can have?
    public float reductionDelta, godwalkerReductionDelta, godwalkerReductionDeltaAdditional; // our rage reduction delta
    public Color startColor, endColor, godwalkingColor; // our start and end colors
    public float maxSpeedBoost, currentSpeedBoost; // how much faster do we move BEFORE entering Godwalker?
    public float godwalkerSpeedBoost; // how fast do we move in Godwalker?
    public float godwalkerTime; // how long do we remain in godwalker?
    public bool godwalking; // oh, it's happening, baby. 
    public GameObject godwalkerVolume; // the godwalker VFX volume
    #endregion


    // public function to add rage
    public void AddRage(float amount)
    {
        if (behaviour == Behaviours.original)
        {
            // make sure we don't go over the maximum
            if (originalRageAmount + amount <= levelGates[levelGates.Count - 1])
                originalRageAmount += amount * levelMods[(int)rageLevel];

            // if we go over, set to maximum
            if (originalRageAmount + amount > levelGates[levelGates.Count - 1])
                originalRageAmount = levelGates[levelGates.Count - 1];
        }

        if (behaviour == Behaviours.v2)
        {
            if (rageAmount + amount <= maxRage)
            {
                rageAmount += amount;
            } else { rageAmount = maxRage; }

            rageAmount = Mathf.Clamp(rageAmount, 0, maxRage);
        }
    }

    private void Update()
    {
        switch (behaviour)
        {
            case Behaviours.original:
                ProcessRage();
                break;

            case Behaviours.v2:
                ProcessRagev2();
                break;
        }
    }

    public void ProcessRage()
    {
        // setup current level
        if (originalRageAmount < levelGates[0])
            rageLevel = RageLevels.benign;

        if (originalRageAmount > levelGates[0] && originalRageAmount < levelGates[1])
            rageLevel = RageLevels.wrecker;

        if (originalRageAmount > levelGates[1] && originalRageAmount < levelGates[2])
            rageLevel = RageLevels.demonic;

        if (originalRageAmount > levelGates[2] && originalRageAmount < levelGates[3])
            rageLevel = RageLevels.eviscerator;

        if (originalRageAmount > levelGates[3])
            rageLevel = RageLevels.godwalker;

        // setup stats and info
        levelName = levelNames[(int)rageLevel];
        levelDelta = levelDeltas[(int)rageLevel];
        sliderImage.color = rageColors[(int)rageLevel];
        rageLevelDisplay.text = levelName;


        // lower rage amount over time
        if (originalRageAmount > 0)
            originalRageAmount -= levelDelta * Time.fixedDeltaTime;
       
        // always update value
        rageSlider.value = originalRageAmount;

        // setup lerp slider
        rageLerp.minValue = rageSlider.minValue;
        rageLerp.maxValue = rageSlider.maxValue;
        rageLerp.value = Mathf.Lerp(rageLerp.value, rageSlider.value, rageLerpSpeed*Time.fixedDeltaTime);

        // setup our values for the slider
        if (rageLevel == RageLevels.benign)
        {
            // min is 0 
            rageSlider.minValue = 0;
            rageSlider.maxValue = levelGates[0];
        } else if (rageLevel != RageLevels.benign)
        {
            // for all other values set minimum to current rage level -1 and max to rage level in level gates
            rageSlider.minValue = levelGates[(int)rageLevel-1];
            rageSlider.maxValue = levelGates[(int)rageLevel];
        }

        // manage our vignette
        if ((int)rageLevel > 0)
        {
            Color ourColor = new Color(rageColors[(int)rageLevel].r, rageColors[(int)rageLevel].g, rageColors[(int)rageLevel].b, 0.5f);
            rageVignette.color = ourColor;
        } else
        {
            rageVignette.color = new Color(0, 0, 0, 0);
        }

        // check for if player has reached godwalker
        if (rageLevel == RageLevels.godwalker)
        {
            GodwalkerTime += Time.fixedDeltaTime;
            reachedGODWALKERDisplay.text = "GODWALKER ACHIEVED FOR " + GodwalkerTime + " SECONDS";
        }

        // process HP regen
        PlayerStatManager.instance.AddHealth(levelHPRegen[(int)rageLevel] * Time.fixedDeltaTime);

    }

    // process our new rage
    public void ProcessRagev2()
    {
        // reduce our rage if we have not maxed out the meter
        if (rageAmount != maxRage && rageAmount > 0 && !godwalking)
        rageAmount -= reductionDelta * Time.fixedDeltaTime;

        // run our UI
        rageSlider.value = rageAmount / maxRage;

        // setup lerp slider
        rageLerp.minValue = rageSlider.minValue;
        rageLerp.maxValue = rageSlider.maxValue;
        rageLerp.value = Mathf.Lerp(rageLerp.value, rageSlider.value, rageLerpSpeed * Time.fixedDeltaTime);

        // lerp the color of our images
        if (!godwalking)
        {
            sliderImage.color = Color.Lerp(startColor, endColor, rageSlider.value);
            rageVignette.color = Color.Lerp(startColor, endColor, rageSlider.value);

            // set our speed boost
            currentSpeedBoost = 1 + maxSpeedBoost * rageAmount / maxRage;

        }

        // if we're at our max rage and we're not godwalking
        if (rageAmount == maxRage && !godwalking)
        {
            rageLevelDisplay.text = "GODWALKER READY PRESS G TO ACTIVATE";
        }

        // entering godwalker - if our bar is full and we press G
        if (Input.GetKeyDown(KeyCode.G) && rageAmount == maxRage)
        {
            // we are now godwalking
            godwalking = true;

        }

        if (godwalking)
        {
            // effects
            godwalkerVolume.SetActive(true);
            flameVFX.SetActive(true);
            // go apeshit
            rageLevelDisplay.text = "GODWALKING"; 
            // set rage vignette to godwalker color
            rageVignette.color = godwalkingColor;
            sliderImage.color = godwalkingColor;
            // reduce our rage by the amount we are spending
            rageAmount -= (godwalkerReductionDelta + godwalkerReductionDeltaAdditional) * Time.fixedDeltaTime;
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
                // reset the delta
                godwalkerReductionDelta = 0;
                // deactivate the fx
                flameVFX.SetActive(false);
                // we are no longer godwalking
                godwalking = false;
                godwalkerVolume.SetActive(false);
                godwalkerReductionDeltaAdditional = 0;
                rageLevelDisplay.text = "";
            }

        }
    }

}
