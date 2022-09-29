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
    { Head, Torso, Arm, Leg }
    // what type of bodypart is this?
    public BodyPartTypes bodyPartType;

    // our cosmetic object information
    public List<GameObject> cosmeticParts; // set list in inspector of our parts
    public Transform cosmeticParent; // the parent of our cosmetic object, used for randomized bodyparts
    public List<GameObject> cosmeticParents; // the list of cosmetic parents, same order as our enum

    // the info about our body part
    public string descriptiveInfo, bodyPartName; // the information about this bodypart in text

    public bool cancelConstruct;

    public void Start()
    {
        PartStart();
        // only run these if we have not canceled our construction
        // construct part
        if (!cancelConstruct)
        {
            ConstructPart(cancelConstruct);
            // check our cosmetics if we do not have a cosmetic parent
            CosmeticSave();
        }
    }

    public void RefreshPart(BodyPartTypes type) {

        this.bodyPartType = type;
        ConstructPart(cancelConstruct);
        // check our cosmetics if we do not have a cosmetic parent
        CosmeticSave();
    }

    // construct our part
    void ConstructPart(bool canceled)
    {  
        // only run if you have parents
        if (cosmeticParents.Count > 0)
        {
            // what type are we?
            if (!canceled)
            {
                int i = Random.Range(0, 4); bodyPartType = (BodyPartTypes)i;
            }
            // setup our cosmetic parts from the cosmetic parent
            cosmeticParent = cosmeticParents[(int)bodyPartType].transform;
            // then remove the parents we dont need
            for (int j = 0; j < cosmeticParents.Count; j++)
            {
                if (cosmeticParents[j].transform != cosmeticParent.transform)
                    Destroy(cosmeticParents[j].gameObject);
            }
        }
    }

    // get our cosmetics
    void CosmeticSave()
    {
        // if we do not have a cosmetic parent, this is not a randomized object, so use our transform
        if (cosmeticParent == null)
        {
            if (cosmeticParts.Count <= 0)
            {
                foreach (Transform child in transform)
                {
                    cosmeticParts.Add(child.gameObject);
                }
            }
        }

        // if we have a cosmetic parent, this is a randomized object, so use our parent
        if (cosmeticParent)
        {
            if (cosmeticParts.Count <= 0)
            {
                foreach (Transform child in cosmeticParent)
                {
                    cosmeticParts.Add(child.gameObject);
                }
            }
        }
    }

    // our start that runs manually after our class start
    public virtual void PartStart()
    {
        //Debug.Log("The part " + gameObject.name + "has a PartStart which has not been overridden");
    }

    public virtual void OnJump() { }                // triggered when the player presses the jump button
    public virtual void OnLand() { }                // triggered when the player lands on the ground 

    public virtual void OnMoveUp() { }              // triggers every frame the player moves upwards
    public virtual void OnMoveDown() { }            // triggers every frame the player moves downwards
    public virtual void OnMoveMidair() { }          // triggeres every frame the player is midair

    public virtual void OnWeaponFire() { }          // triggred when a weapon is used
    public virtual void OnDoubleShot() { }          // triggered whenever a Double shot is fired
    public virtual void OnHomingShot() { }          // triggered when a Homing shot is fired

    public virtual void OnHomingShotDamage() { }    // triggered when a Homing Shot deals damage to an enemy

    public virtual void OnADS() { }                 // triggered when the player ADS
    public virtual void OffADS() { }                // triggered when the player stops ADS
    public virtual void OnSprint() { }              // triggered when the player sprints
    public virtual void OffSprint() { }             // triggered when the player sprints
    public virtual void OnReload() { }              // triggered when the player reloads
    public virtual void OnWeaponSwap() { }          // whenever player changes weapons
    public virtual void OnPlayerTakeDamage() { }    // triggered when the player takes damage
    public virtual void OnBodyPartPickup() { }      // triggered when a body part is picked up
    public virtual void OnProjectileHit() { }       // triggers when a projectile hits an enemy
}