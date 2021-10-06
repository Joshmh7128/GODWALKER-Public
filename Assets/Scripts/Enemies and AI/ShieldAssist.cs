using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAssist : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    private void FixedUpdate()
    {
        transform.position = targetTransform.position;

        if (targetTransform == null)
        {
            Destroy(gameObject);
        }
    }
}
