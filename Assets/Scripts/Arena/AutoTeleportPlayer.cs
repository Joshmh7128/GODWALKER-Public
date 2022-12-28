using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTeleportPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // move the player on start
        PlayerController.instance.Teleport(transform.position);
    }
}
