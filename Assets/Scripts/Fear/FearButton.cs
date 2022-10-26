using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FearButton : MonoBehaviour
{

    [SerializeField] int row, col;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AssignValue);
    }

    void AssignValue()
    {
        FearManager.instance.AssignFear(row, col);
    }
}
