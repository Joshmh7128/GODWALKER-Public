using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : MonoBehaviour
{
    public bool canGrab;
    // can we be picked up?
    public float pickupDistance = 5;

}
