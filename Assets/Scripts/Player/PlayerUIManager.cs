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

    // our instance of the bodypart manager 
    PlayerBodyPartManager partManager;

    // lists concerning bodyparts
    [SerializeField] List<Text> nameDisplays;
    [SerializeField] List<Text> infoDisplays;

    // ui elements
    [SerializeField] CanvasGroup bodyPartCanvasGroup; // the body part canvas group we'll be interacting with

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


    // set our names and info
    private void UpdateUI()
    {

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
