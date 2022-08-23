using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public enum BehaviourType
    {
        movement, attack
    }

    // our public type
    public BehaviourType type;

    // our enemy class
    public EnemyClass enemyClass;

    public void OnEnable()
    {
        // add ourselves to the enemy class
        enemyClass.allBehaviours.Add(this);
    }

    // our main behaviour
    public virtual void RunMain()
    {
        StartCoroutine(MainCoroutine());
    }
    // the coroutine where the behaviour is defined, per class
    public abstract IEnumerator MainCoroutine();

    // our behaviour time
    public float behaviourTime, behaviourTimeRand; // how long it is in seconds, realtime
}
