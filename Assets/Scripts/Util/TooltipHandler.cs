using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipHandler : MonoBehaviour
{
    // the window and text
    [SerializeField] GameObject tooltipPanel, healthBarArrows, godwalkerBarArrows, coinArrow;
    [SerializeField] TextMeshProUGUI tooltipText;
    Vector3 originalPos;
    Vector3 healthPos = new Vector3(-386, -159, 0), godwalkerBar = new Vector3(-240, -193, 0), godCoinPos = new Vector3(226, -173, 0);
    [SerializeField] RectTransform rectTransform;

    // list of things the player has done
    public bool canMove, canJump, canShoot;

    // the tooltips we can do
    public enum Tooltips
    {
        none, tooltip, movement, jumping, shooting, health, godbar, godbar2, godbar3, currency, goodLuck
    }

    Tooltips tabAction;

    private void Start()
    {
        SetTooltip(Tooltips.tooltip);
        originalPos = rectTransform.anchoredPosition;
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
                break;
                

            case (int)Tooltips.tooltip:
                tooltipText.text = "This is a Tooltip. It'll teach you how to play and appear when you learn something new.";
                tabAction = Tooltips.movement;
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
                tooltipText.text = "This is your God Meter. This meter fills when you deal damage or kill enemies.";
                rectTransform.anchoredPosition = godwalkerBar;
                healthBarArrows.SetActive(false);
                godwalkerBarArrows.SetActive(true);
                tabAction = Tooltips.godbar2;
                break;

            case (int)Tooltips.godbar2:
                tooltipText.text = "When this meter is full you can enter Godwalker by pressing G. ";
                tabAction = Tooltips.godbar3;
                break;

            case (int)Tooltips.godbar3:
                tooltipText.text = "While Godwalking you have unlimited ammo and heal. This is the only way to heal.";
                godwalkerBarArrows.SetActive(false);
                tabAction = Tooltips.currency;
                break;

            case (int)Tooltips.currency:
                tooltipText.text = "These are your God coins, get them by killing enemies while Godwalking.";
                coinArrow.SetActive(true);
                rectTransform.anchoredPosition = godCoinPos;
                tabAction = Tooltips.goodLuck;
                break;

            case (int)Tooltips.goodLuck:
                tooltipText.text = "That's all you need to know for now. Good luck.";
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TabAction();

        if (Input.GetKeyDown(KeyCode.T))
        {
            gameObject.SetActive(false);
        }
    }


}
