using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerationManager : MonoBehaviour
{
    // our basic functions for both generation sets
    public abstract void ClearGen();
    public abstract void MapGeneration();
}
