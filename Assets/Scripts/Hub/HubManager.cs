using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    // player spawn tracking
    [SerializeField] GameData gameData;
    // this script saves and tracks all the upgrades the player has obtained
    // currency stored in the hub
    public float hubMineralAmount;
    public float hubGemAmount;
    public float hubBugPartAmount;

    // Start is called before the first frame update
    void Awake()
    {
        // make sure we don't destroy on load
        DontDestroyOnLoad(this);
        // save on load
        gameData.SaveAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
