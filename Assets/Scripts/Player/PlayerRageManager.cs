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
    public float rageAmount; // our total amount of rage

    // UI variables
    public List<Color> rageColors;
    public List<float> levelGates; // where each level of rage ends
    public List<string> levelNames; // the names of each level
    public string levelName; // the name of the current level to display
    public TextMeshProUGUI rageLevelDisplay;
    public List<float> levelDeltas; // how quickly each level reduces
    public List<float> levelMods; // how quickly each level reduces
    public float levelDelta; // our current level delta
    public Slider rageSlider; // our slider
    public Slider rageLerp; // our lerp slider
    public float rageLerpSpeed; // how fast our lerper goes
    public Image sliderImage; // our slider image on our lerper
    public Image rageVignette; // our rage vignette
    public TextMeshProUGUI reachedGODWALKERDisplay; // has this player reached godwalker?
    float GodwalkerTime; // how long we've been in godwalker

    // effect variables
    public List<float> movementMultipliers;
    public List<float> damageMultipliers; 

    // public function to add rage
    public void AddRage(float amount)
    {
        // make sure we don't go over the maximum
        if (rageAmount + amount <= levelGates[levelGates.Count-1])
        rageAmount += amount * levelMods[(int)rageLevel];

        // if we go over, set to maximum
        if (rageAmount + amount > levelGates[levelGates.Count-1])
        rageAmount = levelGates[levelGates.Count-1];
    }

    private void FixedUpdate()
    {
        ProcessRage();
    }

    public void ProcessRage()
    {
        // setup current level
        if (rageAmount < levelGates[0])
            rageLevel = RageLevels.benign;

        if (rageAmount > levelGates[0] && rageAmount < levelGates[1])
            rageLevel = RageLevels.wrecker;

        if (rageAmount > levelGates[1] && rageAmount < levelGates[2])
            rageLevel = RageLevels.demonic;

        if (rageAmount > levelGates[2] && rageAmount < levelGates[3])
            rageLevel = RageLevels.eviscerator;

        if (rageAmount > levelGates[3])
            rageLevel = RageLevels.godwalker;

        // setup stats and info
        levelName = levelNames[(int)rageLevel];
        levelDelta = levelDeltas[(int)rageLevel];
        sliderImage.color = rageColors[(int)rageLevel];
        rageLevelDisplay.text = levelName;


        // lower rage amount over time
        if (rageAmount > 0)
            rageAmount -= levelDelta * Time.fixedDeltaTime;
       
        // always update value
        rageSlider.value = rageAmount;

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
    }


}
