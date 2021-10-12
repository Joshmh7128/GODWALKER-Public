using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldArtifact : ArtifactClass
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
                if (ourRarity == artifactRarities.Common) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + (x = 1); }
                if (ourRarity == artifactRarities.Uncommon) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + (x = Random.Range(2, 3)); }
                if (ourRarity == artifactRarities.Rare) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + (x = Random.Range(2, 5)); }
                artifactName = ourRarity.ToString() + " AutoShield";
                artifactInfo = "If damage is taken shield appears for " + x + " seconds with an " + x * 2 + " cooldown. Stacks. Total: " + UpgradeSingleton.Instance.autoShieldDuration;
                // destroy ourselves
                Pickup();
            }
        }
    }
}
