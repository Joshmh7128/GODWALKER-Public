using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BodyPartClass : MonoBehaviour
{
    /// <summary>
    /// This class serves as the baseline for every body part that the player can use on their body
    /// within this we are able to construct unique parts, and build extension scripts to program new functionality
    /// inside this script all player ability triggers will be openly defined so that they can be defined later in other unique bodypart scripts
    /// </summary>

    // our bodypart types
    public enum BodyPartTypes
    { Head, Torso, Arm, Leg }
    // what type of bodypart is this?
    public BodyPartTypes bodyPartType;

    // define all abstract functions here
    public abstract void OnJump();
}
