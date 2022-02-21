using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ArtifactClass : MonoBehaviour
{
    // what does every artifact have?
    public string artifactName; // our name
    public string artifactInfo; // our info
    public enum artifactRarities // what rarities can we be?
    { Common, Uncommon, Rare }
    public artifactRarities ourRarity; // what is our rarity?
    [SerializeField] Material commonMaterial;
    [SerializeField] Material uncommonMaterial;
    [SerializeField] Material rareMaterial;
    [SerializeField] Renderer rarityMaterialRenderer;
    [SerializeField] LineRenderer ourLine;
    [SerializeField] Sprite ourImage;
    public bool hasCollided = false; // have we collided already?

    private void Awake()
    {
        // select our rarity on awake. choose a number between 1 and 100
        int i = Random.Range(1, 100);
        if (i <= 70) { ourRarity = artifactRarities.Common; rarityMaterialRenderer.material = commonMaterial; }
        if (i >= 71 && i <= 94) { ourRarity = artifactRarities.Uncommon; rarityMaterialRenderer.material = uncommonMaterial; }
        if (i >= 95) { ourRarity = artifactRarities.Rare; rarityMaterialRenderer.material = rareMaterial; }
        ourLine.startColor = rarityMaterialRenderer.material.color;
        ourLine.endColor = rarityMaterialRenderer.material.color;
    }

    // our custom pickup function to be used when the artifact is picked up
    public void Pickup()
    {
        // build and add our info to the list
        UpgradeSingleton.Instance.artifactInfoList.Add(artifactName + " - " + artifactInfo);
        // destroy
        Destroy(gameObject);
    }
}
