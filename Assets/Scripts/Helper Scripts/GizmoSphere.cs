using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSphere : MonoBehaviour
{
    [SerializeField]float size;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
}
