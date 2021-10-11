using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtifactClass : MonoBehaviour
{
    // what does every artifact have?
    public string artifactName; // our name
    public enum artifactRarities // what rarities can we be?
    { common, uncommon, rare }
    public artifactRarities ourRarity; // what is our rarity?
    [SerializeField] Material commonMaterial;
    [SerializeField] Material uncommonMaterial;
    [SerializeField] Material rareMaterial;
    [SerializeField] Renderer rarityMaterialRenderer;
    [SerializeField] LineRenderer ourLine;

    private void Awake()
    {
        // select our rarity on awake. choose a number between 1 and 100
        int i = Random.Range(1, 100);
        if (i <= 70) { ourRarity = artifactRarities.common; rarityMaterialRenderer.material = commonMaterial; }
        if (i >= 71 && i <= 94) { ourRarity = artifactRarities.uncommon; rarityMaterialRenderer.material = uncommonMaterial; }
        if (i >= 95) { ourRarity = artifactRarities.rare; rarityMaterialRenderer.material = rareMaterial; }
        ourLine.startColor = rarityMaterialRenderer.material.color;
        ourLine.endColor = rarityMaterialRenderer.material.color;
    }
}
