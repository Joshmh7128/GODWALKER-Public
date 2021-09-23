using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// our non-JSON SaveData
[Serializable]
public class SaveData
{
    // create a public float list
    public float[] SaveDataFloatArray = new float[(int)GameData.SaveDataTypes.saveDataEnumMax];
}
