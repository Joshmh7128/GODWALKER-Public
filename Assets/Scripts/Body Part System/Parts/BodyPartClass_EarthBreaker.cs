using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartClass_EarthBreaker : BodyPartClass
{
    [SerializeField] GameObject groundProjectile; // the projectile we launch in cardinal direction

    // when we land fire some projectiles
    public override void OnLand()
    {
        PlayerController player = PlayerController.instance;
        // on land, instantiate a ground projectile
        Instantiate(groundProjectile, player.transform.position, Quaternion.identity, null);
    }
}
