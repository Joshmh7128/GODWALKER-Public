using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour_SetActive : EnemyBehaviour
{
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();

    public override IEnumerator MainCoroutine()
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(true);
        }

        yield return null;

    }
}
