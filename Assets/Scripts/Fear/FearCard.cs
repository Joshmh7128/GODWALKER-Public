using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FearCard : MonoBehaviour
{
    /// this card is used to display the current level of fear on the player and then either add a new 
    /// detriment or advanced a current one
    /// 

    // our fear manager
    FearManager fearManager;

    public enum fearTypes
    {
        movementSpeed, 
        jumping, 
        dashing,
        shootingSpeed, 
        deflecting,
        health, 
        projectileSpeed
    }

    // our active type
    public List<fearTypes> fears = new List<fearTypes>();

    // our modes
    public enum cardModes { increase, decrease }
    public cardModes cardMode; // what mode is this card in?

    public TextMeshProUGUI infoTextDisplay;
    public string infoText; // our information

    private void Start()
    {
        // get our instance
        fearManager = FearManager.instance;

        StartCoroutine(StartBuffer());
    }

    IEnumerator StartBuffer()
    {
        yield return new WaitForFixedUpdate();       
        // grab info
        InfoGrab();
    }

    void InfoGrab()
    {
        // get the info of what this card will do
        if (cardMode == cardModes.increase)
        {
            foreach (fearTypes fear in fears)
            {
                // get info of this fear in its next stage
                 // previous effect is replaced with new effect

                infoText += "\n" + "- You now have: " + fearManager.effects[(int)fear].effectInfos[fearManager.effects[(int)fear].effectStage] + ", this card replaces it with: " + fearManager.effects[(int)fear].effectInfos[fearManager.effects[(int)fear].effectStage+1] + "\n";
            }

            // then update
            infoTextDisplay.text = infoText;
        }
    }

    // when this card is picked up, choose and apply its effects
    void Pickup()
    {
        if (cardMode == cardModes.increase)
        {
            // advance our types
            foreach (fearTypes fear in fears)
            {
                // advance it in the correct fear category
                fearManager.effects[(int)fear].IncreaseEffect();
                fearManager.effects[(int)fear].ApplyEffect();
            }
        }

        if (cardMode == cardModes.decrease)
        {
            // advance our types
            foreach (fearTypes fear in fears)
            {
                // advance it in the correct fear category
                fearManager.effects[(int)fear].DecreaseEffect();
                fearManager.effects[(int)fear].ApplyEffect();
            }
        }

        // then destroy this
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Debug.Log("pickup");
            Pickup();
        }
    }

}
