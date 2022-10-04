using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIHandler : MonoBehaviour
{
    /// <summary>
    /// this script manages an individual ability UI element that goes in the 
    /// </summary>

    public BodyPartClass bodyPart;
    public Slider chargeSlider; // our charge slider
    [SerializeField] AudioSource readySource; // the sound we play when we're recharged

    public void UseAbility()
    {
        // setup charge slider
        chargeSlider.maxValue = bodyPart.abilityRechargeTimeMax;
        chargeSlider.value = bodyPart.abilityRechargeTime;

        readySource.Play();
        Debug.Log("Ability used");
    }

    private void FixedUpdate()
    {
        
    }

}
