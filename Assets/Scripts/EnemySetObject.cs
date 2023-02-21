using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Some personal notes about enemy encounters:
/// Room 1: Basic crabs and orbs
/// Room 2: Basic crabs, orbs, and missile launchers
/// Room 3: Basic crabs, orbs, missile launchers, centaurs
/// Room 4: Crabs, Orbs, missiles, centaurs, torrents
/// Room 5: crabs, orbs, missiles, centaurs, torrents, helion
/// Room 6: crabs, orbs, missiles, centaurs, torrents, helion, boom crabs
/// </summary>

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemySetObject", order = 1)]
public class EnemySetObject : ScriptableObject
{
    // setup the list of encounters
    public List<EnemySetList> roomList = new List<EnemySetList>();
}

[System.Serializable]
public class EnemySetList
{
    public List<EnemySet> encounterList = new List<EnemySet>();
}

[System.Serializable]
public class EnemySet
{
    public List<Wave> waveList = new List<Wave>();
}

[System.Serializable]
public class Wave
{
    public List<GameObject> enemyList = new List<GameObject>();   
}


