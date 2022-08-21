using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyClass : MonoBehaviour
{
    // class exists as the baseline for all of our enemies

    // our behaviours
    public List<EnemyBehaviour> allBehaviours;
    List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();

    // our agent
    public NavMeshAgent navMeshAgent;

    private void Start()
    {
        // sort our behaviours
        SortBehaviours();
        // start them 
        StartBehaviours();
    }

    // to run our behaviours
    void StartBehaviours()
    {
        // start our behaviours
        StartCoroutine(AttackBehaviourHandler());
        StartCoroutine(MovementBehaviourHandler());
    }

    IEnumerator AttackBehaviourHandler()
    {
        // go through each attack
        foreach (EnemyBehaviour behaviour in attackBehaviours)
        {
            // run the behaviour
            behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
        }

        StartCoroutine(AttackBehaviourHandler());
    }
    
    IEnumerator MovementBehaviourHandler()
    {
        // go through each attack
        foreach (EnemyBehaviour behaviour in movementBehaviours)
        {
            // run the behaviour
            behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
        }

        StartCoroutine(MovementBehaviourHandler());
    }

    // to sort the behaviours that our body uses
    void SortBehaviours()
    {
        foreach (EnemyBehaviour behaviour in allBehaviours)
        {
            if (behaviour.type == EnemyBehaviour.BehaviourType.attack)
            { attackBehaviours.Add(behaviour); }

            if (behaviour.type == EnemyBehaviour.BehaviourType.movement)
            { movementBehaviours.Add(behaviour); }
        }
    }

}
