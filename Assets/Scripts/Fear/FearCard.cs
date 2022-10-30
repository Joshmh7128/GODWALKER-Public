using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearCard : MonoBehaviour
{
    /// this card is used to display the current level of fear on the player and then either add a new 
    /// detriment or advanced a current one
    /// 

    // our fear manager
    FearManager fearManager;

    public enum fearTypes
    {
        movementSpeed, 
        jumping, 
        dashing,
        shootingSpeed, 
        deflecting,
        health, 
        projectileSpeed
    }

    private void Start()
    {
        // get our instance
        fearManager = FearManager.instance;
    }

    // when this card is picked up, choose and apply its effects
    void Pickup()
    {

    }

}
