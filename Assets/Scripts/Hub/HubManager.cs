using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    // player spawn tracking
    public GameObject activePlayer;
    public GameObject activePlayerPackage;
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
        // make sure to add our keys
        if (!PlayerPrefs.HasKey("hubMineralAmount"))
        { PlayerPrefs.SetFloat("hubMineralAmount", hubMineralAmount); }
        else { PlayerPrefs.SetFloat("hubMineralAmount", hubMineralAmount); }

        if (!PlayerPrefs.HasKey("hubGemAmount"))
        { PlayerPrefs.SetFloat("hubGemAmount", hubGemAmount); }
        else { PlayerPrefs.SetFloat("hubGemAmount", hubGemAmount); }

        if (!PlayerPrefs.HasKey("hubBugPartAmount"))
        { PlayerPrefs.SetFloat("hubBugPartAmount", hubBugPartAmount); }
        else { PlayerPrefs.SetFloat("hubBugPartAmount", hubBugPartAmount); }
        // make sure we save our changes
        PlayerPrefs.Save();
    }

    // Call this when we want to save our updated PlayerPrefs
    void SavePrefs()
    {
        // make sure we save our changes
        PlayerPrefs.Save();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
