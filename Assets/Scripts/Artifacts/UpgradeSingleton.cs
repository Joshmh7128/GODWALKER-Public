using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSingleton : ScriptableObject
{
    private static UpgradeSingleton _instance;

    public static UpgradeSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UpgradeSingleton.CreateInstance<UpgradeSingleton>();
                Debug.Log("UpgradeSingleton Instance created");
            }
            return _instance;
        }
    }

    public static void DestroySingleton()
    {
        Destroy(_instance);
    }

    // basic trackers
    public bool playerPlaced; // should always be true. here as an initial get
    public PlayerController player; // our player, set by our player controller
    public List<string> artifactInfoList = new List<string>(); // the list of our artifact info per artifact
    // upgrade related values that can be accessed everywhere
    public float autoShieldDuration;

    

}
