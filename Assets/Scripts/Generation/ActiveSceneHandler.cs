using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveSceneHandler : MonoBehaviour
{
    /// <summary>
    /// script will set the scene it is placed in to active
    /// </summary>
    /// 
    void Start()
    {
        SceneManager.SetActiveScene(gameObject.scene);
    }
}
