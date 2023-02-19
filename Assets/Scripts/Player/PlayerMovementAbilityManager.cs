using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAbilityManager : MonoBehaviour
{
    /// <summary>
    /// This script holds the booleans for each movement ability, and is used
    /// to track which ones are active or inactive
    /// </summary>
    
    // setup instance
    public static PlayerMovementAbilityManager instance;
    private void Awake()
    {
        instance = this;
    }

    // movement abilities are programmed on the player controller itself
    // these booleans are set to true when the player picks up a movement ability
    [Header("Ability Tracking")]
    public bool dashActive;
    public bool tripleJumpActive, hoverActive; 

}
