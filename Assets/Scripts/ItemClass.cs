using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : MonoBehaviour
{
    public abstract void ShowInfo();
    public abstract void HideInfo();
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

}
