using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] // run this in the editor
public class CustomGizmo : MonoBehaviour
{
    // just a helper script to assist with making gizmos
    enum GizmoTypes { sphere, wireSphere, cube, wireCube}
    [SerializeField] GizmoTypes type; // the type of this gizmo
    [SerializeField] float size; // the size of this gizmo
    [SerializeField] Color color; // our color

    // our ondraw gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (type == GizmoTypes.sphere)
        {
            Gizmos.DrawSphere(transform.position, size);
        }

        if (type == GizmoTypes.wireSphere)
        {
            Gizmos.DrawWireSphere(transform.position, size);
        }      
        
        if (type == GizmoTypes.cube)
        {
            Gizmos.DrawCube(transform.position, new Vector3(size,size,size));
        }

        if (type == GizmoTypes.wireCube)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
        }
    }


}
