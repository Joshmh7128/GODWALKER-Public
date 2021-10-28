using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryArtifactUIHandler : MonoBehaviour
{
    // our prefab object
    [SerializeField] GameObject artifactUIGridElementPrefab;
    [SerializeField] GameObject artifactIconUIGridPrefab;
    // our grid parent object
    [SerializeField] Transform gridParent;
    [SerializeField] Transform iconGridParent;
    // all the objects in that grid object
    public List<ArtifactUIGridElementClass> artifactUIGridElementClasses;
    public List<GameObject> artifactMiniIcons;
    // our UI elements
    public Text selectedTitleText;
    public Text selectedInfoText;

    // are we adding something new to the grid?
    public void UpdateInventoryGrid(string titleText, string infoText, Sprite icon)
    {
        // make our object and add it to the grid
        ArtifactUIGridElementClass newElement;
        newElement = Instantiate(artifactUIGridElementPrefab, gridParent).GetComponent<ArtifactUIGridElementClass>();
        
        // make sure the information on our newElement is correct
        newElement.gameObject.transform.SetAsFirstSibling();
        newElement.artifactIcon.sprite = icon;
        newElement.artifactInfoText = infoText;
        newElement.artifactTitleText.text = titleText;
        newElement.selectionTitleText = selectedTitleText;
        newElement.selectionInfoText = selectedInfoText;
        // add it to our list
        artifactUIGridElementClasses.Add(newElement);
        
        // add our icon
        GameObject artifactMiniIcon;
        artifactMiniIcon = Instantiate(artifactIconUIGridPrefab, iconGridParent);
        artifactMiniIcon.GetComponent<Image>().sprite = icon;
    }

    public void ClearInventoryGrid()
    {
        foreach (ArtifactUIGridElementClass element in artifactUIGridElementClasses)
        {
            Destroy(element.gameObject);
        }       
        
        foreach (GameObject element in artifactMiniIcons)
        {
            Destroy(element.gameObject);
        }

        selectedInfoText.text = "No Artifact Selected";
        selectedTitleText.text = "No Artifact Selected";

        artifactUIGridElementClasses.Clear();
    }
}
