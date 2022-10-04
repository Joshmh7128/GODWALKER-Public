using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    /// this script is used to manage all of the player's UI outside of the weapons
    /// it was created to manage the body part display
    /// 
    /// for all our lists of bodyparts, here's the order
    /// <summary>
    /// 0 - head
    /// 1 - torso
    /// 2 - right arm
    /// 3 - left arm 
    /// 4 - right leg
    /// 5 - left leg
    /// </summary>

    // setup our instance
    public static PlayerUIManager instance;
    private void Awake() => instance = this;

    // our instance of the bodypart manager 
    PlayerBodyPartManager partManager;

    // lists concerning bodyparts
    [SerializeField] List<Text> nameDisplays;
    [SerializeField] List<Text> infoDisplays;
    [SerializeField] List<Image> backgroundPanels; // all the background panels for the body

    // ui elements
    [SerializeField] CanvasGroup bodyPartCanvasGroup; // the body part canvas group we'll be interacting with
    [SerializeField] HorizontalLayoutGroup abilityLayoutGroup; // our ability layout group


    // start
    private void Start()
    {
        // set the instance of this
        partManager = PlayerBodyPartManager.instance;
    }

    private void FixedUpdate()
    {
        // all things we need to process related to our input
        ProcessInput();
    }

    // update our ability UI
    public void UpdateAbilityUI()
    {
        // clear our current ability group if we have an ability group
        try
        {
            foreach (Transform child in abilityLayoutGroup.transform)
            {
                Destroy(child);
            }
        }
        catch { return; }


        // check all our parts for ability prefabs
        foreach (var part in partManager.bodyParts)
        {
            if (part.abilityCosmetic != null)
            {
                // spawn in that prefab if this upgrade has an ability on it into the ui group
                AbilityUIHandler element = Instantiate(part.abilityCosmetic, abilityLayoutGroup.transform).GetComponent<AbilityUIHandler>();

                // link an associated body part to the ability cosmetic ui
                element.bodyPart = part;
                part.abilityCosmetic = element;
            } else if (part.abilityCosmetic == null) { return; }
        }
    }

    // set our names and info
    private void UpdateBodyPartUI()
    {
        // set all names and info to the correct text
        for (int i = 0; i < nameDisplays.Count; i++)
        {
            nameDisplays[i].text = partManager.bodyParts[i].bodyPartName; // set the name
            infoDisplays[i].text = partManager.bodyParts[i].descriptiveInfo; // set the descriptive information

            // set the correlating background display to the average color of the bodypart
            Color panelColor = Color.white;
            foreach (GameObject cos in partManager.bodyParts[i].cosmeticParts)
            {
                Color cosColor = Color.white;
                Color temp = Color.white;
                // get the average color of the parts in the cosmeticpart parent
                foreach (Transform child in cos.transform)
                {
                    if (child.GetComponent<Renderer>() != null)
                    {

                        // set our temp to the color
                        temp.r = child.gameObject.GetComponent<Renderer>().sharedMaterial.color.r;
                        temp.g = child.gameObject.GetComponent<Renderer>().sharedMaterial.color.g;
                        temp.b = child.gameObject.GetComponent<Renderer>().sharedMaterial.color.b;
                        // lerp the color
                        cosColor = Color.Lerp(cosColor, temp, 0.5f);
                        cosColor = Color.Lerp(cosColor, Color.white, 0.25f); // lerp back to white for visibility
                    }
                }

                // get the color and lerp it
                panelColor = Color.Lerp(panelColor, cosColor, 0.5f);
            }
            // once we lerp to all the colors set the panel color
            backgroundPanels[i].color = panelColor;

        }
    }

    // process our inputs
    private void ProcessInput()
    {
        // whenever we press tab, show the entire body part canvas
        if (Input.GetKey(KeyCode.Tab))
        {
            ShowCanvas();
        } else
        {
            HideCanvas();
        }
    }

    // show our canvas
    private void ShowCanvas()
    {
        bodyPartCanvasGroup.alpha = 1;
        // update our information whenever we show it
        UpdateBodyPartUI();
    }

    // fading out our canvas
    private void HideCanvas()
    {
        if (bodyPartCanvasGroup.alpha > 0)
        {
            bodyPartCanvasGroup.alpha -= 0.05f;
        }
    }

}
