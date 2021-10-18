using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactUIGridElementClass : MonoBehaviour
{
    // stores everything that an upgrade needs to display and passes it to the UI handler
    public Text artifactTitleText;
    public string artifactInfoText;
    public Image artifactIcon;
    public Text selectionTitleText;
    public Text selectionInfoText;
    [SerializeField] Button ourButton;

    private void Start()
    {
        if (artifactTitleText != null)
        {
            if (artifactTitleText.ToString().Contains("Filler"))
            {

            }

            if (artifactTitleText.ToString().Contains("Uncommon"))
            {
                ColorBlock uncommonColors = new ColorBlock();
                uncommonColors.normalColor = new Color(136, 255, 136, 255);
                uncommonColors.selectedColor = new Color(130, 226, 129, 255);
                uncommonColors.pressedColor = new Color(104, 195, 104, 255);
                ourButton.colors = uncommonColors;
            }

            if (artifactTitleText.ToString().Contains("Rare"))
            {
                ColorBlock rareColors = new ColorBlock();
                rareColors.normalColor = new Color(121, 199, 255, 255);
                rareColors.selectedColor = new Color(117, 179, 231, 255);
                rareColors.pressedColor = new Color(85, 135, 231, 255);
                ourButton.colors = rareColors;
            }
        }
    }

    public void LocalOnClick()
    {
        selectionTitleText.text = artifactTitleText.text;
        selectionInfoText.text = artifactInfoText;
    }
}
