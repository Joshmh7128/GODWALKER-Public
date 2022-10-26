using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearManager : MonoBehaviour
{
    // this script is used to manage the fear levels and changes how the player works when fear is applied
    // it houses all the stats and displays them

    // build and assign instance
    static FearManager instance;
    private void Awake() => instance = this;

    /// all of our fear stats
    
    // movement speed
    public enum moveSpeeds { full, halved, third, quarter };
    public moveSpeeds moveSpeed;
    // jumps
    public enum jumpAmount {  }

}
