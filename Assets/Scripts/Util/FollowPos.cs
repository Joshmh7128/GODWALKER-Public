using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPos : MonoBehaviour
{
    [SerializeField] Transform targetTransform; // our target
    [SerializeField] bool unparent; // should we unparent

    private void OnEnable()
    {
        if (unparent)
            transform.Unparent();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform)
        transform.FollowPos(targetTransform);
    }
}
