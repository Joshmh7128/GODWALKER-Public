using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerGodfeelManager : MonoBehaviour
{
    // script exists to manage our gamefeel in godwalker mode
    [SerializeField] PlayerRageManager rageManager; // our rage manager

    [SerializeField] List<Volume> volumes; // all the different volumes which can become kill volumes
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

    public void ChooseVolume(int level)
    {
        // reset volume weight
        foreach (Volume volume in volumes) volume.weight = 0;
        // set the current kill volume
        killVolume = volumes[level];
    }

    // kick it.
    public void KickFeel()
    {
        killVolume.weight = 1;
    }
}
