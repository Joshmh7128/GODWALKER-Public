using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    /// this script is used to manage all of the player's UI outside of the weapons
    /// it was created to manage the body part display
    /// 
    /// for all our lists of bodyparts, here's the order
    /// <summary>
    /// 0 - head
    /// 1 - torso
    /// 2 - right arm
    /// 3 - left arm 
    /// 4 - right leg
    /// 5 - left leg
    /// </summary>


    // lists concerning bodyparts
    [SerializeField] List<Text> nameDisplays;
    [SerializeField] List<Text> infoDisplays;

}
