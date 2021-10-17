using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryArtifactUIHandler : MonoBehaviour
{
    // our prefab object
    [SerializeField] GameObject artifactUIGridElementPrefab;
    // our grid parent object
    [SerializeField] Transform gridParent;
    // all the objects in that grid object
    public List<ArtifactUIGridElementClass> artifactUIGridElementClasses;

    // are we adding something new to the grid?
    public void UpdateInventoryGrid(string titleText, string infoText, Sprite icon)
    {
        // make our object and add it to the grid
        ArtifactUIGridElementClass newElement;
        newElement = Instantiate(artifactUIGridElementPrefab, gridParent).GetComponent<ArtifactUIGridElementClass>();
        // make sure the information on our newElement is correct
        newElement.artifactIcon.sprite = icon;
        newElement.artifactInfoText = infoText;
        newElement.artifactTitleText.text = titleText;
        // add it to our list
        artifactUIGridElementClasses.Add(newElement);
    }

    public void ClearInventoryGrid()
    {
        artifactUIGridElementClasses.Clear();
    }
}
