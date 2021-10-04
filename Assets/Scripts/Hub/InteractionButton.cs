using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class InteractionButton : MonoBehaviour
{
    public float mineralCost;
    public float gemCost;
    public float bugPartCost;
    bool mouseIn = false;
    Player player;
    public bool canInteract = false;
    [SerializeField] Button ourButton;
    [SerializeField] HubLink hubLink;
    public Color idleColor;
    public Color hoverColor;
    public Color clickColor;
    [SerializeField] Image ourImage;
    [SerializeField] Text panelTitle;
    [SerializeField] Text panelDesc;
    [SerializeField] Text mineralText;
    [SerializeField] Text gemText;
    [SerializeField] Text bugPartText;
    public string customTitle;
    public string customDesc;
    public enum buttonTypes
    {
        dropshipMineralStorageButton,
        dropshipGemStorageButton,
        dropshipAmmoStorageButton,
        dropshipBugPartStorageButton,
        playerMineralStorageButton,
        playerGemStorageButton,
        playerAmmoStorageButton,
        playerBugPartStorageButton,
    }

    public buttonTypes buttonType;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void FixedUpdate()
    {
        if (hubLink == null)
        {
            hubLink = GameObject.Find("HubLink").GetComponent<HubLink>();
        }

        if (mouseIn)
        {
            ourImage.color = hoverColor;
        } else
        {
            ourImage.color = idleColor;
        }
        
        if (player.GetButtonDown("Fire"))
        {
            if (mouseIn == true && canInteract == true)
            {
                ourImage.color = clickColor;
                ourButton.onClick.Invoke();
            }
        }

        // make sure we have a hubmanager before attempting this
        if (hubLink != null)
        {
            if (hubLink.hubManager != null)
            {
                if (buttonType == buttonTypes.dropshipAmmoStorageButton)
                {
                    bugPartCost = hubLink.hubManager.droppodManager.ammoUpgradeCost;
                }

                if (buttonType == buttonTypes.dropshipGemStorageButton)
                {
                    bugPartCost = hubLink.hubManager.droppodManager.gemUpgradeCost;
                }

                if (buttonType == buttonTypes.dropshipMineralStorageButton)
                {
                    bugPartCost = hubLink.hubManager.droppodManager.mineralUpgradeCost;
                }

                if (buttonType == buttonTypes.dropshipBugPartStorageButton)
                {
                    bugPartCost = hubLink.hubManager.droppodManager.bugPartUpgradeCost;
                }

                if (bugPartText != null)
                {
                    bugPartText.text = bugPartCost.ToString();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = true;
            panelTitle.text = customTitle;
            panelDesc.text = customDesc;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = false;
            panelTitle.text = " ";
            panelDesc.text = " ";
        }
    }

    public void ButtonTestFunction(string text)
    {
        Debug.Log("DEBUG TEST FUNCTION: " + text);
    }
}
