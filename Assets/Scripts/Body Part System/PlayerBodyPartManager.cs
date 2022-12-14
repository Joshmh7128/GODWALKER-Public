using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyPartManager : MonoBehaviour
{
    /// <summary>
    /// This script serves as the manager for all six of our bodyparts
    /// head, torso, right arm, left arm, right leg, left leg
    /// 
    /// each of these has their own slot, which is tracked here. 
    /// the instance of this script is called by any other scripts to interact with the functions on the associated parts.
    /// 
    /// </summary>

    // setup and set our instance
    public static PlayerBodyPartManager instance;
    public PlayerUIManager uiManager;
    private void Awake() 
    {
        instance = this;
        uiManager = PlayerUIManager.instance;
    }

    public float pickupCooldown; // our pickup cooldown

    public GameObject highlightedBodyPart;

    private void Start()
    {
        // make sure we refresh parts when this object is created
        RefreshParts();
    }

    // all the information needed to use our cosmetic info when we pickup parts

    // head
    public List<Transform> headPartParents;

    // torso
    public List<Transform> torsoPartParents;

    // legs
    public List<Transform> rightLegPartParents;
    public List<Transform> leftLegPartParents;

    // arms
    public List<Transform> rightArmPartParents;
    public List<Transform> leftArmPartParents;

    // our bodypart classes
    public BodyPartClass headPartClass, torsoPartClass, rightArmPartClass, leftArmPartClass, rightLegPartClass, leftLegPartClass;

    // the list of our 6 bodyparts
    public List<BodyPartClass> bodyParts = new List<BodyPartClass>();
    /// <summary>
    /// 0 - head
    /// 1 - torso
    /// 2 - arm
    /// 4 - leg
    /// </summary>

    public void CallParts(string function)
    {
        foreach (BodyPartClass bodyPartClass in bodyParts)
        {
            // Debug.Log(function);
            if (bodyPartClass)
            bodyPartClass.Invoke(function,0f);
        }
    }

    public void CallParts(string function, float time)
    {
        foreach (BodyPartClass bodyPartClass in bodyParts)
        {
            // Debug.Log(function);
            bodyPartClass.Invoke(function, time);
        }
    }

    // refresh parts - makes sure we spawn in new parts when we start
    void RefreshParts()
    {
        // refresh our cosmetics
        PickupPart(headPartClass, BodyPartClass.BodyPartTypes.Head);
        PickupPart(torsoPartClass, BodyPartClass.BodyPartTypes.Torso);
        PickupPart(rightArmPartClass, BodyPartClass.BodyPartTypes.Arm, true);
        PickupPart(leftArmPartClass, BodyPartClass.BodyPartTypes.Arm, false);
        PickupPart(rightLegPartClass, BodyPartClass.BodyPartTypes.Leg, true);
        PickupPart(leftLegPartClass, BodyPartClass.BodyPartTypes.Leg, false);
    }

    // our part pickup class
    public void PickupPart(BodyPartClass part, BodyPartClass.BodyPartTypes type)
    {
        // swap our type and react accordingly //

        // if this is a head...
        if (type == BodyPartClass.BodyPartTypes.Head)
        {
            // for every parent...
            foreach (Transform parent in headPartParents)
            {
                // check their children and destory them
                foreach (Transform child in parent)
                { Destroy(child.gameObject); } 
            }

            // then put parts back in there...
            for (int i = 0; i < headPartParents.Count; i++)
            {   // get the correlating part and instnatiate it
                if (part.cosmeticParts[i] != null)
                Instantiate(part.cosmeticParts[i], headPartParents[i]);
            }

            // call the ondrop of the body part we are removing
            try { bodyParts[0].OnDropThisPart(); } catch { }

            // put this into our list
            bodyParts[0] = part;

        }

        // if this is a torso...
        if (type == BodyPartClass.BodyPartTypes.Torso)
        {
            // for every parent...
            foreach (Transform parent in torsoPartParents)
            {
                // check their children and destory them
                foreach (Transform child in parent)
                { Destroy(child.gameObject); }
            }

            // then put parts back in there...
            for (int i = 0; i < torsoPartParents.Count; i++)
            {   // get the correlating part and instnatiate it
                Instantiate(part.cosmeticParts[i], torsoPartParents[i]);
            }
            // call the ondrop of the body part we are removing
            try { bodyParts[1].OnDropThisPart(); } catch { }
            // put this into our list
            bodyParts[1] = part;

        }

        // then update the ability UI
        UpdateAbilityUI();
    }

    public void PickupPart(BodyPartClass part, BodyPartClass.BodyPartTypes type, bool isRight)
    {

        // if this is an arm...
        if (type == BodyPartClass.BodyPartTypes.Arm)
        {
            if (isRight)
            {
                // for every parent...
                foreach (Transform parent in rightArmPartParents)
                {
                    // check their children and destory them
                    foreach (Transform child in parent)
                    { Destroy(child.gameObject); }
                }

                // then put parts back in there...
                for (int i = 0; i < rightArmPartParents.Count; i++)
                {   // get the correlating part and instnatiate it
                    Instantiate(part.cosmeticParts[i], rightArmPartParents[i]);
                }

                // call the ondrop of the body part we are removing
                try { bodyParts[2].OnDropThisPart(); } catch { }

                // put this into our list
                bodyParts[2] = part;

            }

            // for left arms
            if (!isRight)
            {
                // for every parent...
                foreach (Transform parent in leftArmPartParents)
                {
                    // check their children and destory them
                    foreach (Transform child in parent)
                    { Destroy(child.gameObject); }
                }

                // then put parts back in there...
                for (int i = 0; i < leftArmPartParents.Count; i++)
                {   // get the correlating part and instnatiate it
                    Instantiate(part.cosmeticParts[i], leftArmPartParents[i]);
                }

                // call the ondrop of the body part we are removing
                try { bodyParts[3].OnDropThisPart(); } catch { }

                // put this into our list
                bodyParts[3] = part;
            }
        }

        // if this is a Leg...
        if (type == BodyPartClass.BodyPartTypes.Leg)
        {
            if (isRight)
            {
                // for every parent...
                foreach (Transform parent in rightLegPartParents)
                {
                    // check their children and destory them
                    foreach (Transform child in parent)
                    { Destroy(child.gameObject); }
                }

                // then put parts back in there...
                for (int i = 0; i < rightLegPartParents.Count; i++)
                {   // get the correlating part and instnatiate it
                    Instantiate(part.cosmeticParts[i], rightLegPartParents[i]);
                }

                // call the ondrop of the body part we are removing
                try { bodyParts[4].OnDropThisPart(); } catch { }

                // put this into our list
                bodyParts[4] = part;
            }

            if (!isRight)
            {
                // for every parent...
                foreach (Transform parent in leftLegPartParents)
                {
                    // check their children and destory them
                    foreach (Transform child in parent)
                    { Destroy(child.gameObject); }
                }

                // then put parts back in there...
                for (int i = 0; i < leftLegPartParents.Count; i++)
                {   // get the correlating part and instnatiate it
                    Instantiate(part.cosmeticParts[i], leftLegPartParents[i]);
                }

                // call the ondrop of the body part we are removing
                try { bodyParts[5].OnDropThisPart(); } catch { }

                // put this into our list
                bodyParts[5] = part;
            }
        }

        // then update the ability UI
        UpdateAbilityUI();
    }

    // tell our player UI manager to update the ability UI
    void UpdateAbilityUI()
    {
        // Debug.Log("Updating ability UI");
        uiManager.UpdateAbilityUI();
    }

    private void FixedUpdate()
    {
        ProcessPickupCooldown();
    }

    // our pickup cooldown
    void ProcessPickupCooldown()
    {
        if (pickupCooldown >= 0)
        {
            pickupCooldown--;
        }
    }

}
