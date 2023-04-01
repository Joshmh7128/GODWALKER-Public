using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    // setup our instance
    public static PlayerUIManager instance;
    private void Awake() => instance = this;

    // our instance of the bodypart manager 
    PlayerBodyPartManager partManager;
    PlayerWeaponManager weaponManager;

    // ui elements
    [SerializeField] CanvasGroup infoCanvasGroup; // the body part canvas group we'll be interacting with
    [SerializeField] HorizontalLayoutGroup abilityLayoutGroup; // our ability layout group
    [SerializeField] GameObject extraCanvasGroup; // our extra canvas group
    [SerializeField] Slider escFillSlider; // shows how long it takes to reset the game to the main menu
    [SerializeField] GameObject mainMenuResetPrefab; // prefab we use to reset to the main menu

    // start
    private void Start()
    {
        // set the instance of this
        partManager = PlayerBodyPartManager.instance;
        weaponManager = PlayerWeaponManager.instance;
    }

    private void FixedUpdate()
    {
        // all things we need to process related to our input
        ProcessInput();
    }

    // update our ability UI
    public void UpdateAbilityUI()
    {
        StartCoroutine(BufferCheck());
    }

    public void UpdateAbilityUI(bool overload)
    {
        // clear our current ability group if we have an ability group
        for (int i = abilityLayoutGroup.transform.childCount - 1; i >= 0; i--)
            Destroy(abilityLayoutGroup.transform.GetChild(i).gameObject);

        
        // check all our parts for ability prefabs
        foreach (BodyPartClass part in partManager.bodyParts)
        {
            if (part.abilityCosmetic != null)
            {
                // spawn in that prefab if this upgrade has an ability on it into the ui group
                AbilityUIHandler element = Instantiate(part.abilityCosmetic, abilityLayoutGroup.transform).GetComponent<AbilityUIHandler>();

                // link an associated body part to the ability cosmetic ui
                element.bodyPart = part;
                part.activeAbilityCosmetic = element;
            }
            else if (part.abilityCosmetic == null) { }
        }
    }

    IEnumerator BufferCheck()
    {
        yield return new WaitForFixedUpdate();
        UpdateAbilityUI(true);        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            escFillSlider.value += Time.deltaTime;
        }
        else
        {
            if (escFillSlider.value > 0)
                escFillSlider.value -= Time.fixedDeltaTime;
        }

        // if we ever hit value...
        if (escFillSlider.value >= 4.9f)
        {
            // reset the game
            Instantiate(mainMenuResetPrefab);
            // set mouse behaviour
            Cursor.lockState = CursorLockMode.None;

            StartCoroutine(BufferKill());
        }
    }

    IEnumerator BufferKill()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(PlayerController.instance.gameObject);
    }

    // process our inputs
    private void ProcessInput()
    {
        // whenever we press tab, show the entire body part canvas
        if (Input.GetKey(KeyCode.Tab))
        {
            try { ShowCanvas(); } catch { }
        } else
        {
            try { HideCanvas(); } catch { }
        }
    }

    // show our canvas
    private void ShowCanvas()
    {
        infoCanvasGroup.alpha = 1;
    }

    // fading out our canvas
    private void HideCanvas()
    {
        if (infoCanvasGroup.alpha > 0)
        {
            infoCanvasGroup.alpha -= 0.05f;
        }
    }

}
