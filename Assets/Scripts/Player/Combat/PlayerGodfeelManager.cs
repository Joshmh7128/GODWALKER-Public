using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerGodfeelManager : MonoBehaviour
{
    // script exists to manage our gamefeel in godwalker mode
    [SerializeField] PlayerRageManager rageManager; // our rage manager

    [SerializeField] Volume killVolume;
    [SerializeField] float weightReductDelta;

    // setup our instance
    public static PlayerGodfeelManager instance;
    private void Awake() => instance = this;

    public void FixedUpdate()
    {
        if (killVolume.weight > 0)
            killVolume.weight -= weightReductDelta * Time.fixedDeltaTime;
    }

    // kick it.
    public void KickFeel()
    {
        killVolume.weight = 1;
    }
}
