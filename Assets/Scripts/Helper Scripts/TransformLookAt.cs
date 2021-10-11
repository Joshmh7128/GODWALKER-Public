using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLookAt : MonoBehaviour
{
    [SerializeField] Transform targetPos;
    [SerializeField] bool targetPlayer;
    // Update is called once per frame
    void Update()
    {
        if (targetPlayer && targetPos == null)
        {
            targetPos = GameObject.Find("Player").GetComponent<Transform>();
        }

        transform.LookAt(targetPos);
    }
}
