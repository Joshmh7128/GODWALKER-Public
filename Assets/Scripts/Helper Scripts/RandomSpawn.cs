using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;
    bool hasInit = false; // have we initiatlized?
    private void OnEnable()
    {
        if (!hasInit)
        {
            // get random number
            float i = Random.Range(0, (float)prefabList.Count);
            // instantiate the thing
            Instantiate(prefabList[(int)i], transform.position, Quaternion.identity, null);
            hasInit = true;
        }
    }
}
