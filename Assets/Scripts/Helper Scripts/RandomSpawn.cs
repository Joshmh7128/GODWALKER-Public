using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;
    bool hasInit = false; // have we initiatlized?
    [SerializeField] bool randomizeRotation; // should we randomize rotation?

    private void OnEnable()
    {
        if (!hasInit)
        {
            // get random number
            float i = Random.Range(0, (float)prefabList.Count);
            // instantiate the thing
            if (randomizeRotation)
            { Instantiate(prefabList[(int)i], transform.position, Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), null); }
            else
            { Instantiate(prefabList[(int)i], transform.position, Quaternion.identity, null); }
            hasInit = true;
        }
    }
}
