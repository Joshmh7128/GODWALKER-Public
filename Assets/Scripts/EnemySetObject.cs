using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemySetObject", order = 1)]
public class EnemySetObject : ScriptableObject
{
    // setup the lists
    public List<EnemySetList> enemySets = new List<EnemySetList>();

    public float test;

}

[System.Serializable]
public class EnemySetList
{
    public List<EnemySet> enemySetList = new List<EnemySet>();
}

[System.Serializable]
public class EnemySet
{
    public List<GameObject> waves = new List<GameObject>();
}

