using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelutionElementActivate : LevelutionElement
{
    public override void ActivateElement()
    {
        gameObject.SetActive(true);
    }
}
