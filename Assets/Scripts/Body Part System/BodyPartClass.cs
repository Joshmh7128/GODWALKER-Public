using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [HideInInspector] public bool overrideGen; // do we override the generation?
    // our cosmetic object information
    public List<GameObject> cosmeticParts; // set list in inspector of our parts
    public Transform cosmeticParent; // the parent of our cosmetic object, used for randomized bodyparts
    [Header("!! ORDER: HEAD, TORSO, ARM, LEG !!")]
    public List<GameObject> cosmeticParents; // the list of cosmetic parents, same order as our enum

    [Header("Ability related")]
    public AbilityUIHandler abilityCosmetic; // our ability UI element
    [HideInInspector] public AbilityUIHandler activeAbilityCosmetic; // our ability UI element
    public float abilityRechargeTime, abilityRechargeTimeMax; // our ability recharge time

    [Header("Information related")]
    // the info about our body part
    [TextArea(1, 20)]
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

        // zero out this part. this function is only used for parts we pickup, not cosmetic parts
        foreach(GameObject part in cosmeticParts)
        {
            part.GetComponent<ZeroOut>().ManualZero();  // manually zero out 
        }
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
                // if we are NOT override generation
                if (!overrideGen)
                {
                    int i = Random.Range(0, 4);
                    bodyPartType = (BodyPartTypes)i;
                }
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

        CosmeticCheck();
    }

    // warning out
    void CosmeticCheck()
    {
        if (cosmeticParents.Count > 0)
        {
            bool check = false;
            string headCheck = cosmeticParents[0].ToString();
            if (!headCheck.Contains("Head")) check = true;
            string torsoCheck = cosmeticParents[1].ToString();
            if (!torsoCheck.Contains("Torso")) check = true;
            string armCheck = cosmeticParents[2].ToString();
            if (!armCheck.Contains("Arm")) check = true;
            string legCheck = cosmeticParents[3].ToString();
            if (!legCheck.Contains("Leg")) check = true;

            if (check)
            {
                Debug.LogError("ERROR: " + gameObject + " HAS INCORRECTLY ASSIGNED OR NAMED COSMETIC PARENTS");
            }
        }
    }

    // fixed update
    private void FixedUpdate()
    {
        // if we used an ability, lower the charge
        if (abilityRechargeTime > 0) abilityRechargeTime -= Time.deltaTime;
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

    public virtual void OnExplosionDamage() { }     // triggered when an explosion deals damage to one enemy
    public virtual void OnExplosionDamagePlayer() { }       // triggered when an explosion deals damage to one enemy
    public virtual void OnMultipleExplosionDamage() { }     // triggered when an explosion deals damage to one enemy
    public virtual void OnShockDamage() { }         // triggered when an explosion deals damage to one enemy

    public virtual void UseAbility()  // direct action non-trigger used to run the ability on a part
    {
        // use our ability
        if (CanUseAbility())
        {
            OnUseAbility();
            // try and use our ability
            try { activeAbilityCosmetic.UseAbility(); } catch { return; }
            // set our timer
            abilityRechargeTime = abilityRechargeTimeMax;
        }
 
    }            

    public bool CanUseAbility() { if (abilityRechargeTime <= 0) { return true; } else return false; }
    public virtual void OnUseAbility() { }          // anything we want to have happen when an ability use is triggered

    public virtual void OnADS() { }                 // triggered every frame the player is aiming down sights
    public virtual void OffADS() { }                // triggered every frame the player is not aiming down sights

    public virtual void TryTeleport(Vector3 targetPos) { }           // used for any parts which want to involve teleportation

    public virtual void OnSprint() { }              // triggered when the player sprints
    public virtual void OffSprint() { }             // triggered when the player sprints
    public virtual void OnReload() { }              // triggered when the player reloads
    public virtual void OnWeaponSwap() { }          // whenever player changes weapons
    public virtual void OnPlayerTakeDamage() { }    // triggered when the player takes damage
    public virtual void OnPlayerGainLife() { }      // triggered when the player gains life
    public virtual void OnBodyPartPickup() { }      // triggered when a body part is picked up
    public virtual void OnProjectileHit() { }       // triggers when a projectile hits an enemy
}