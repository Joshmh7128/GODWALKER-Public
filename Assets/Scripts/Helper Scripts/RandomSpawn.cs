using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;
    bool hasInit = false; // have we initiatlized?
    [SerializeField] bool randomizeRotation; // should we randomize rotation?
    [SerializeField] bool isEnemyGroup; // is this an enemy group?
    [SerializeField] RoomClass ourRoom; // what room is this being spawned in?
    private void OnEnable()
    {
        if (!hasInit)
        {
            // get random number
            float i = Random.Range(0, (float)prefabList.Count);
            // save the object
            GameObject ourObject;
            // instantiate the thing
            if (randomizeRotation)
            { ourObject = Instantiate(prefabList[(int)i], transform.position, Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), null); }
            else
            { ourObject = Instantiate(prefabList[(int)i], transform.position, Quaternion.identity, null); }
            hasInit = true;
        }
    }
}
