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

    public void LocalOnClick()
    {
        selectionTitleText.text = artifactTitleText.text;
        selectionInfoText.text = artifactInfoText;
    }
}
