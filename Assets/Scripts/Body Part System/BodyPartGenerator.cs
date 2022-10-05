using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartGenerator : MonoBehaviour
{
    public GameObject itemPrefab;

    public List<GameObject> partPrefabs;


    public List<Component> partComponents = new List<Component>();

    [SerializeField] GameObject constructedObject;

    private void Start()
    {
        
        BodyPartItem item = Instantiate(itemPrefab).GetComponent<BodyPartItem>();
        item.bodyPartObject = partPrefabs[Random.Range(0, partPrefabs.Count)];

    }

}
