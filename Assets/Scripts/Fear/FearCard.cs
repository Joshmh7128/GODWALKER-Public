using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements.Experimental;

public class FearCard : MonoBehaviour
{
    /// this card is used to display the current level of fear on the player and then either add a new 
    /// detriment or advanced a current one
    /// 

    // our fear manager
    [SerializeField] FearManager fearManager;

    [SerializeField] bool random; // is this card random?
    [SerializeField] int priority, sisterChecksAllowed; // the priority of this card's decision
    [SerializeField] List<FearCard> sisterCards = new List<FearCard>();

    bool canPickup;
    [SerializeField] GameObject highlightPanel; // the panel that shows when this card is the one we are selecting

    public enum fearTypes
    {
        movementSpeed, 
        jumping, 
        dashing,
        shootingSpeed, 
        deflecting,
        health, 
        projectileSpeed,

        Count // our max
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
        // randomize
        RandomizeEffects();
        // grab info
        InfoGrab();
        // wait again
        yield return new WaitForFixedUpdate();
        // sister check
        SisterCheck();
    }

    void InfoGrab()
    {

        // get our instance
        fearManager = FearManager.instance;


        // get the info of what this card will do
        if (cardMode == cardModes.increase)
        {
            foreach (fearTypes fear in fears)
            {
                // get info of this fear in its next stage
                // previous effect is replaced with new effect

                if (fearManager.effects[(int)fear].effectStage + 1 <= fearManager.effects[(int)fear].maxStage)
                {
                    infoText = fearManager.effects[(int)fear].effectInfos[fearManager.effects[(int)fear].effectStage + 1];
                } else  { StartCoroutine(StartBuffer()); }

            }

            // then update
            infoTextDisplay.text = infoText;
        }
        
        // get the info of what this card will do
        if (cardMode == cardModes.decrease)
        {
            foreach (fearTypes fear in fears)
            {
                // get info of this fear in its next stage
                // previous effect is replaced with new effect

                if (fearManager.effects[(int)fear].effectStage - 1 <= fearManager.effects[(int)fear].maxStage)
                {
                    infoText = fearManager.effects[(int)fear].effectInfos[fearManager.effects[(int)fear].effectStage - 1];
                } else  { StartCoroutine(StartBuffer()); }

            }

            // then update
            infoTextDisplay.text = infoText;
        }
    }

    void SisterCheck()
    {
        sisterChecksAllowed++;

        if (sisterChecksAllowed < 600) foreach (FearCard sister in sisterCards)
        {
            // if our sister card has the same fears as us, change it up
            for (int i = 0; i < fears.Count; i++)
            {
                // see if we can access this 
                try 
                { 
                    if (fears[i] == sister.fears[i])
                        {
                            if (priority < sister.priority)
                            {
                                StartCoroutine(StartBuffer());
                                break;
                            }
                        }
                    }
                catch { }

                
            }
        }
    }

    // randomize our values
    void RandomizeEffects()
    {
        List<fearTypes> copiedEffects = new List<fearTypes> {
        fearTypes.movementSpeed,
        fearTypes.jumping,
        fearTypes.dashing,
        fearTypes.shootingSpeed,
        fearTypes.deflecting,
        fearTypes.health,
        fearTypes.projectileSpeed }; // make a list to add our randomized effects to

        // randomize the value of the fear
        for (int i = 0; i < fears.Count; i++)
        {
            fearTypes pulledEffect = copiedEffects[Random.Range(0, copiedEffects.Count)];
            copiedEffects.Remove(pulledEffect);
            fears[i] = pulledEffect;
        }


    }

    // when this card is picked up, choose and apply its effects
    void Pickup()
    {

        // get our instance
        fearManager = FearManager.instance;

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

    private void FixedUpdate()
    {
        highlightPanel.SetActive(canPickup);

        if (canPickup && Input.GetKey(KeyCode.E))
        {
            Pickup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canPickup = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canPickup = false;
        }
    }

}
