using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartUpgradeManager : MonoBehaviour
{
    /// this script manages all of our body parts
    /// 

    // setup our instance
    public static BodyPartUpgradeManager instance;
    private void Awake()
    {
        instance = this;
    }

}
