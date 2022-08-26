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
    public enum BodyPartTypes // we use these arguments to pass information into the bodypart manager for pickups
    { Head, Torso, Arm, Leg}
    // what type of bodypart is this?
    public BodyPartTypes bodyPartType;

    // our cosmetic object information
    public List<GameObject> cosmeticParts; // set list in inspector of our parts

    // the info about our body part
    public string descriptiveInfo, bodyPartName; // the information about this bodypart in text

    public void Start()
    {
        PartStart();
    }

    // our start that runs manually after our class start
    public virtual void PartStart()
    {
        //Debug.Log("The part " + gameObject.name + "has a PartStart which has not been overridden");
    }

    public virtual void OnJump() { }    // triggered when the player presses the jump button
    public virtual void OnLand() { }    // triggered when the player lands on the ground 
    public virtual void OnADS() { }     // triggered when the player ADS
    public virtual void OffADS() { }    // triggered when the player stops ADS
    public virtual void OnSprint() { }  // triggered when the player sprints
    public virtual void OffSprint() { }  // triggered when the player sprints
    public virtual void OnReload() { }  // triggered when the player reloads

}
