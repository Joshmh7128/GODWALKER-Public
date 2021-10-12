using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraLightGogglesArtifact : ArtifactClass
{
    private void OnTriggerEnter(Collider collision)
    {
        // if this hits the player
        if (collision.CompareTag("Player"))
        {
            if (!hasCollided)
            {
                // ensure we don't multi-collide
                hasCollided = true;
                // our randomized int
                int x = 0;
                // determine our modifications based on our rarities
                if (ourRarity == artifactRarities.Common) { UpgradeSingleton.Instance.tetralightVisionAddition = UpgradeSingleton.Instance.tetralightVisionAddition + (x = Random.Range(1, 4)); }
                if (ourRarity == artifactRarities.Uncommon) { UpgradeSingleton.Instance.tetralightVisionAddition = UpgradeSingleton.Instance.tetralightVisionAddition + (x = Random.Range(3, 8)); }
                if (ourRarity == artifactRarities.Rare) { UpgradeSingleton.Instance.tetralightVisionAddition = UpgradeSingleton.Instance.tetralightVisionAddition + (x = Random.Range(7, 10)); }
                artifactName = ourRarity.ToString() + " TetraLight Goggles";
                artifactInfo = "On Kill gain enemy highlights through all walls for " + x + " seconds. Stacks.";
                // destroy ourselves
                Pickup();
            }
        }
    }
}
