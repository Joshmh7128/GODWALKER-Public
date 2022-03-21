using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSceneList : MonoBehaviour
{
    /// <summary> 
    /// This script is used in a list in out LayoutList
    /// It holds a list of scenes which can be referenced by a generation manager
    /// </summary>
    /// 

    public List<Scene> chunks; // the chunks that can be spawned in our room
}
