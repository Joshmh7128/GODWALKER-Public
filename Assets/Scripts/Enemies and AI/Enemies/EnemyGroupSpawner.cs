using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupSpawner : EnemyClass
{
    [SerializeField] List<GameObject> prefabList;
    bool hasInit = false; // have we initiatlized?
    [SerializeField] bool randomizeRotation; // should we randomize rotation?
    [SerializeField] float randomRadius;
    private void OnEnable()
    {
        if (!hasInit)
        {
            // get random number
            float i = Random.Range(0, (float)prefabList.Count);
            // save the object
            GameObject ourObject;
            Vector3 randomAddition = new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius));
            // instantiate the thing
            if (randomizeRotation)
            { ourObject = Instantiate(prefabList[(int)i], transform.position + randomAddition, Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0)), null); }
            else
            { ourObject = Instantiate(prefabList[(int)i], transform.position + randomAddition, Quaternion.identity, null); }
            hasInit = true;
            ourObject.GetComponent<EnemyClass>().roomClass = roomClass;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, randomRadius);
    }

    public override void TakeDamage(int dmg)
    {
        throw new System.NotImplementedException();
    }
}