using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallChunkManager : MonoBehaviour
{
    public void ClearChildren()
    {
        int previousCount = transform.childCount;

        for (int i = 0; i < previousCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
