using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class TooltipHandler : MonoBehaviour
{
    // the window and text
    [SerializeField] GameObject tooltipPanel, healthBarArrows, godwalkerBarArrows, coinArrow;
    [SerializeField] TextMeshProUGUI tooltipText;
    Vector3 originalPos;
    Vector3 healthPos = new Vector3(-386, -159, 0), godwalkerBar = new Vector3(-240, -193, 0), godCoinPos = new Vector3(226, -173, 0);
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Image background, showcase;
    [SerializeField] AudioSource closePip; // our audio closing pip
    [SerializeField] List<Sprite> imageList; // the list of our images

    bool shownElementalPop; // have we shown an elemental popup to the player?

    public static TooltipHandler instance; // our nonstatic instance
    private void Awake()
    {
        instance = this;
    }

    // the tooltips we can do
    public enum Tooltips
    {
        none, tooltip, movement, jumping, shooting, health, godbar, godbar2, godbar3, currency, goodLuck, 

        elementalPop, normalDMG, weakDMG, strongDMG,


        juicePop
    }

    Tooltips tabAction;

    private void Start()
    {
        // disable our showcase image
        showcase.enabled = false;
        originalPos = rectTransform.anchoredPosition;
        SetTooltip(Tooltips.tooltip);
    }

    // use this function to set a tooltip
    public void SetTooltip(Tooltips desiredTip)
    {
        // activate the panel
        tooltipPanel.SetActive(true);

        switch ((int)desiredTip)
        {
            case (int)Tooltips.none:
                tooltipPanel.SetActive(false);
                healthBarArrows.SetActive(false);
                godwalkerBarArrows.SetActive(false);
                coinArrow.SetActive(false);
                background.enabled = false;
                showcase.enabled = false;
                break;
                
            case (int)Tooltips.tooltip:
                tooltipText.text = "This is a Tooltip. It'll teach you how to play and appear when you learn something new.";
                tooltipPanel.SetActive(true);
                rectTransform.anchoredPosition = originalPos;
                tabAction = Tooltips.movement;
                background.enabled = true;
                break;

            case (int)Tooltips.movement:
                tooltipText.text = "Use the WASD keys to move around";
                tabAction = Tooltips.jumping;
                break;

            case (int)Tooltips.jumping:
                tooltipText.text = "Use Space to Jump. You can double jump by jumping while midair.";
                tabAction = Tooltips.shooting;
                break;

            case (int)Tooltips.shooting:
                tooltipText.text = "Look around with the mouse. Use Left Click to shoot.";
                tabAction = Tooltips.health;
                break;

            case (int)Tooltips.health:
                tooltipText.text = "This is your health bar. If you lose all of this, you die.";
                rectTransform.anchoredPosition = healthPos;
                healthBarArrows.SetActive(true);
                tabAction = Tooltips.godbar;
                break;

            case (int)Tooltips.godbar:
                tooltipText.text = "This is your God Juice Meter. This meter fills when you deal damage or kill enemies.";
                rectTransform.anchoredPosition = godwalkerBar;
                healthBarArrows.SetActive(false);
                godwalkerBarArrows.SetActive(true);
                tabAction = Tooltips.godbar2;
                break;

            case (int)Tooltips.godbar2:
                tooltipText.text = "When this meter is full you can enter Godmode by pressing G. ";
                tabAction = Tooltips.godbar3;
                break;

            case (int)Tooltips.godbar3:
                tooltipText.text = "While Godwalking you deal extra damage and heal. This is the only way to heal.";
                godwalkerBarArrows.SetActive(false);
                tabAction = Tooltips.currency;
                break;

            case (int)Tooltips.currency:
                tooltipText.text = "These are your God coins, get them by killing enemies while in Godmode.";
                coinArrow.SetActive(true);
                rectTransform.anchoredPosition = godCoinPos;
                tabAction = Tooltips.goodLuck;
                break;

            case (int)Tooltips.goodLuck:
                tooltipText.text = "You can hold 3 weapons, and cycle them by scroll wheel or pressing R. Now escape. Good luck.";
                coinArrow.SetActive(false);
                rectTransform.anchoredPosition = originalPos;
                tabAction = Tooltips.none;
                break;
                
            case (int)Tooltips.elementalPop:
                if (!shownElementalPop)
                {
                    tooltipPanel.SetActive(true); // make sure we set this to active because this is triggered externally
                    background.enabled = true;
                    shownElementalPop = true; // make sure we log that we have shown this
                    tooltipText.text = "You'll be facing enemies with elemental weaknesses soon. Choose your weapon wisely!";
                    rectTransform.anchoredPosition = originalPos;
                    tabAction = Tooltips.normalDMG; // show our normal damage number explanation next
                }
                break;


            case (int)Tooltips.normalDMG:
                tooltipPanel.SetActive(true); // make sure we set this to active because this is triggered externally
                background.enabled = true;
                tooltipText.text = "White damage numbers indicate normal damage.";
                rectTransform.anchoredPosition = originalPos;
                showcase.enabled = true;
                showcase.sprite = imageList[0];
                tabAction = Tooltips.strongDMG; // show our normal damage number explanation next
                break;

            case (int)Tooltips.strongDMG:
                tooltipPanel.SetActive(true); // make sure we set this to active because this is triggered externally
                background.enabled = true;
                tooltipText.text = "Red damage numbers indicate bonus damage. Correct elements deal more damage.";
                rectTransform.anchoredPosition = originalPos;
                showcase.sprite = imageList[1];
                tabAction = Tooltips.weakDMG; // show our normal damage number explanation next
                break;

            case (int)Tooltips.weakDMG:
                tooltipPanel.SetActive(true); // make sure we set this to active because this is triggered externally
                background.enabled = true;
                tooltipText.text = "Yellow damage numbers indicate weak damage. Incorrect elements deal less damage.";
                rectTransform.anchoredPosition = originalPos;
                showcase.sprite = imageList[2];
                tabAction = Tooltips.none; // show our normal damage number explanation next
                break;

            case (int)Tooltips.juicePop:
                tooltipText.text = "The longer you hold on to weapons the less God Juice they generate. Check this in the right-hand panel by switching weapons.";
                coinArrow.SetActive(false);
                rectTransform.anchoredPosition = originalPos;
                tabAction = Tooltips.none;
                break;
        }
    }

    void TabAction()
    {
        tooltipPanel.SetActive(false);
        SetTooltip(tabAction);
        closePip.Play();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("f1");
            SetTooltip(Tooltips.tooltip);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            TabAction();

        if (Input.GetKeyDown(KeyCode.T))
        {
            SetTooltip(Tooltips.none);
        }
    }


}
