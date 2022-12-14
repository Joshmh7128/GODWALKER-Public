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

    // our completion
    public bool complete;

    // our enemy class
    public EnemyClass enemyClass;

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
