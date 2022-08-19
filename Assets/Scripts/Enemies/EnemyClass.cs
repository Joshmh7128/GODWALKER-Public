using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{
    // class exists as the baseline for all of our enemies

    // our behaviours
    [SerializeField] List<EnemyBehaviour> allBehaviours;
    List<EnemyBehaviour> attackBehaviours = new List<EnemyBehaviour>();
    List<EnemyBehaviour> movementBehaviours = new List<EnemyBehaviour>();

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

        // go through each attack
        foreach (EnemyBehaviour behaviour in movementBehaviours)
        {
            // run the behaviour
            behaviour.RunMain();
            // then wait
            yield return new WaitForSecondsRealtime(behaviour.behaviourTime);
        }
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
