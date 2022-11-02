using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_ChangePhase : EnemyBehaviour
{
    public override IEnumerator MainCoroutine()
    {
        enemyClass.currentPhase++;
        enemyClass.SortBehaviours(enemyClass.phaseParents[enemyClass.currentPhase]);

        yield break; 
    }
}
