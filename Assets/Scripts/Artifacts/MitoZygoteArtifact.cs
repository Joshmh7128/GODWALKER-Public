using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitoZygoteArtifact : ArtifactClass
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
                if (ourRarity == artifactRarities.Common) { UpgradeSingleton.Instance.mitoZygoteAddition = UpgradeSingleton.Instance.mitoZygoteAddition + (x = 1); }
                if (ourRarity == artifactRarities.Uncommon) { UpgradeSingleton.Instance.mitoZygoteAddition = UpgradeSingleton.Instance.mitoZygoteAddition + (x = Random.Range(2, 3)); }
                if (ourRarity == artifactRarities.Rare) { UpgradeSingleton.Instance.mitoZygoteAddition = UpgradeSingleton.Instance.mitoZygoteAddition + (x = 4); }
                artifactName = ourRarity.ToString() + " MitoZygote";
                artifactInfo = "When you pick up Scrap gain a 1 HP shield for " + x + " seconds. All values stack.";
                // destroy ourselves
                Pickup();
            }
        }
    }
}
