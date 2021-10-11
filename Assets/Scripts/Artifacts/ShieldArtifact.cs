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
            // determine our modifications based on our rarities
            if (ourRarity == artifactRarities.common) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + 1; }
            if (ourRarity == artifactRarities.uncommon) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + (int)Random.Range(1, 3); }
            if (ourRarity == artifactRarities.rare) { UpgradeSingleton.Instance.autoShieldDuration = UpgradeSingleton.Instance.autoShieldDuration + (int)Random.Range(2, 5); }
            // destroy ourselves
            Destroy(gameObject);
        }
    }
}
