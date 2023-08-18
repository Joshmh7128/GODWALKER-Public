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
    [SerializeField] List<float> rageReductionDeltas; // how quickly our rage depletes as we godwalk
    public float reductionDelta, originalReductionDelta, godwalkerReductionDelta, godwalkerReductionDeltaAdditional, originalAdditional; // our rage reduction delta
    public Color startColor, endColor, godwalkingColor; // our start and end colors
    public float maxSpeedBoost, currentSpeedBoost; // how much faster do we move BEFORE entering Godwalker?
    public float godwalkerSpeedBoost; // how fast do we move in Godwalker?
    public float godwalkerTime; // how long do we remain in godwalker?
    public bool godmoding; // oh, it's happening, baby. 
    public GameObject godwalkerVolume; // the godwalker VFX volume
    bool godwalkMusicRequestMade; // have we made our godwalker music request?


    public TextMeshProUGUI rageLevelDisplay;
    #endregion

    [Header("V3 elements")]
    public Slider backSlider; // our slider representing how far away we are from the next level
    public List<Color> rageColors;
    public List<float> ammoRechargeMods; // how quickly ammo refills
    [SerializeField] RectTransform showPoint, normPoint; // for moving text around on the screen
    bool canReturn; // can the rage text return yet?

    // setup rage levels
    public enum RageLevels
    {
        CRAWLER,
        JOGGER,
        RUNNER,
        SPRINTER,
        WALKER
    }

    public RageLevels rageLevel; // which rage level we're currently on

    public float overRage, currentWeaponOverRageCoefficient; // rage that exceeds the godwalker meter
    [SerializeField] List<float> overRageGates; // how much overrage we have to build to get to the next level, each starts from 0
    [SerializeField] List<float> overRageDeltas; // how much overrage we lost as we go
    public List<float> damageMult; // our damage multipliers

    private void Start()
    {
        originalReductionDelta = godwalkerReductionDelta; // set the original delta
        originalAdditional = godwalkerReductionDeltaAdditional;

    }

    // public function to add rage
    public void AddRage(float amount, bool isKill = false)
    {
        if (rageAmount + amount <= maxRage)
        {
            rageAmount += amount;
        } else { rageAmount = maxRage; }

        if (godmoding)
        {
            // add to our overRage to increase our godwalking level
            overRage += amount;
            
            /// then heal our HP at specific rates while in this mode
            // if it is not a kill, add 2 HP per shot hit
            if (!isKill)
                PlayerStatManager.instance.AddHealth(1f);

            // if it is a kill, add 15 HP
            if (isKill)
                PlayerStatManager.instance.AddHealth(10f);

        }

        rageAmount = Mathf.Clamp(rageAmount, 0, maxRage);
    }

    private void Update()
    {
        // process inputs
        ProcessInput();
    }

    private void FixedUpdate()
    {
        ProcessRage();
    }

    // process our new rage
    public void ProcessRage()
    {
        // reduce our rage if we have not maxed out the meter
        if (rageAmount != maxRage && rageAmount > 0 && !godmoding)
        {
            rageAmount -= reductionDelta * Time.fixedDeltaTime;
        }

        if (overRage > 0 && !godmoding)
        {
            overRage -= reductionDelta * Time.fixedDeltaTime * currentWeaponOverRageCoefficient;
        }

        // process our overRage
        if (overRage > overRageGates[(int)rageLevel])
        {
            // set our rage to max when we level up
            rageAmount = maxRage;
            if ((int)rageLevel < 4)
            {
                // raise the rage level
                rageLevel++;

                // show pop 
                LevelPop();

                // set the volume
                PlayerGodfeelManager.instance.ChooseVolume((int)rageLevel);

                // kick the feel
                PlayerGodfeelManager.instance.KickFeel();
            }

            overRage = 0; // reset it
        }

        // process our UI 
        ProcessUI();

        // process godwalker
        ProcessGodmode();
    }

    void ProcessGodmode()
    {
        if (godmoding)
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
            rageAmount -= rageReductionDeltas[(int)rageLevel] * Time.fixedDeltaTime * currentWeaponOverRageCoefficient;
            // reduce our overrage
            if (overRage > 0)
            overRage -= overRageDeltas[(int)rageLevel] * Time.fixedDeltaTime * currentWeaponOverRageCoefficient;
            // refill HP
            // PlayerStatManager.instance.AddHealth(20 * Time.fixedDeltaTime);
            // if we're godwalking raise our speedboost
            currentSpeedBoost = 1 + godwalkerSpeedBoost * ((float)rageLevel / (float)RageLevels.WALKER);

            // refill all our weapon ammo 
            if (rageLevel == RageLevels.WALKER)
            {
                foreach (GameObject weapon in PlayerWeaponManager.instance.weapons)
                {
                    // refill so we are using maximum ammo on all weapons
                    weapon.GetComponent<WeaponClass>().currentMagazine = weapon.GetComponent<WeaponClass>().maxMagazine;
                    // make the music request
                    if (!godwalkMusicRequestMade)
                    {
                        godwalkMusicRequestMade = true;
                        SimpleMusicManager.instance.desiredMood = SimpleMusicManager.MusicMoods.godwalking;
                    }
                }
            }

            // godwalker time
            godwalkerTime += Time.deltaTime;

            // when do we end godwalking?
            if (rageAmount <= 0)
            {
                godwalkMusicRequestMade = false;
                // reset the delta & additional
                godwalkerReductionDelta = originalReductionDelta; // reset
                godwalkerReductionDeltaAdditional = originalAdditional;
                // deactivate the fx
                flameVFX.SetActive(false);
                // we are no longer godwalking
                godmoding = false;
                godwalkerVolume.SetActive(false);
                rageLevelDisplay.text = "";
                // reset the color
                rageLevelDisplay.color = Color.white;
                // reset the rage level
                rageLevel = 0;
                // reset display
                rageLevelDisplay.rectTransform.position = normPoint.position;
                rageLevelDisplay.rectTransform.localScale = Vector3.one;

                // check if this is the best time
                if (godwalkerTime > PlayerRunStatTracker.instance.longestGodwalkerTime)
                    PlayerRunStatTracker.instance.longestGodwalkerTime = (int)godwalkerTime;

                // reset time
                godwalkerTime = 0;

                // reset music only if we are still in combat
                if (!ArenaManager.instance.activeArena.combatComplete)
                    SimpleMusicManager.instance.desiredMood = SimpleMusicManager.MusicMoods.combat;

            }

        }

        if (!godmoding)
        {
            // set our speed boost
            currentSpeedBoost = 1;
        }
    }

    void ProcessInput()
    {

        // entering godwalker - if our bar is full and we press G
        if (Input.GetKeyDown(KeyCode.G) && rageAmount == maxRage)
        {
            if (!godmoding)
                // kick feel
                PlayerGodfeelManager.instance.KickFeel();

            LevelPop();

            // we are now godwalking
            godmoding = true;

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
        backSlider.value = overRage;

        // colors while godwalking
        sliderImage.color = Color.Lerp(startColor, endColor, rageSlider.value);
        rageVignette.color = Color.Lerp(startColor, endColor, rageSlider.value);

        // lerp the color of our images
        if (!godmoding)
        {
            sliderImage.color = Color.Lerp(startColor, endColor, rageSlider.value);
            rageVignette.color = Color.Lerp(startColor, endColor, rageSlider.value);
        }

        // if we're at our max rage and we're not godwalking
        if (rageAmount == maxRage && !godmoding)
        {
            rageLevelDisplay.text = "PRESS G TO ACTIVATE";
            LevelPop();
        }

        // move the text display back
        if (canReturn)
        {
            rageLevelDisplay.rectTransform.position = Vector3.Lerp(rageLevelDisplay.rectTransform.position, normPoint.position, 3 * Time.fixedDeltaTime);
            rageLevelDisplay.rectTransform.localScale = Vector3.Lerp(rageLevelDisplay.rectTransform.localScale, Vector3.one, 3 * Time.fixedDeltaTime);
        }
    }

    void LevelPop()
    {
        canReturn = false;

        // set text color
        rageLevelDisplay.color = rageColors[(int)rageLevel];

        // set text dimensions
        rageLevelDisplay.rectTransform.position = showPoint.position;
        rageLevelDisplay.rectTransform.localScale = Vector3.one * 2;
        StartCoroutine(ReturnBuffer());
    }

    IEnumerator ReturnBuffer()
    {
        yield return new WaitForSecondsRealtime(2f);
        canReturn = true;
    }


    // add to the delta
    IEnumerator ReductionDeltaIncreaseCheck()
    {
        // if the player maintains godwalker for 20 seconds
        yield return new WaitForSecondsRealtime(20f);
        // increase the rage by 10%
        if (godmoding)
            originalAdditional += 2.5f * 0.1f;
    }

}
