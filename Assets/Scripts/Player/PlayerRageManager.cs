using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<float> levelDeltas; // how quickly each level reduces
    public float levelDelta; // our current level delta
    public Slider rageSlider; // our slider
    public Image sliderImage; // our slider image


    // public function to add rage
    public void AddRage(float amount)
    {
        rageAmount += amount;
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

        // lower rage amount over time
        if (rageAmount > 0)
            rageAmount -= levelDelta * Time.fixedDeltaTime;
       
        // always update value
        rageSlider.value = rageAmount;

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
    }


}
