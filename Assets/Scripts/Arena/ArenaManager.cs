using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    /// this class serves as a manager for a game-wide arena manager
    /// access this instance to get the active arena
    /// 

    // setup our instance for ease of access
    public static ArenaManager instance;
    private void Awake()
    { instance = this; }

    public ArenaHandler activeArena; // the arena which the player most recently activated

    // our upgrade sets to be used by our arenas when we spawn in upgrades
    public List<GameObject> mainSet, alternateSet, specialSet; // the main, alternate, and special sets
    public int mainIndex, alternateIndex, specialIndex;


}
