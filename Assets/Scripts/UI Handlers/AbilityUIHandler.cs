using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIHandler : MonoBehaviour
{
    /// <summary>
    /// this script manages an individual ability UI element that goes in the 
    /// </summary>

    public BodyPartClass bodyPart;
    public float chargeTime;
    [SerializeField] AudioSource readySource; // the sound we play when we're recharged

    public void UseAbility()
    {

    }

}
