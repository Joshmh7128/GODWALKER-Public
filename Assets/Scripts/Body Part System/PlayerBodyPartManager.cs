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
    /// </summary>

    // setup and set our instance
    public static PlayerBodyPartManager instance;
    private void Awake()
    { instance = this; }

    // all the information needed to use our cosmetic info when we pickup parts

    // head
    public GameObject headPartParent;

    // torso
    public GameObject neckPartParent, TorsoSpine0Parent, TorsoSpine1Parent, TorsoSpine2Parent;

    // legs
    public GameObject rightUpperLegParent, rightLowerLegParent, rightFootParent;
    public GameObject leftUpperLegParent, leftLowerLegParent, leftFootParent;

    // arms
    public GameObject rightShoulderParent, rightArmParent, rightForearmParent, rightHandParent;
    public GameObject leftShoulderParent, leftArmParent, leftForearmParent, leftHandParent;
}
