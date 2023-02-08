using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


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


