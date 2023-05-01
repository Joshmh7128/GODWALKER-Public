using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualArenaStarter : MonoBehaviour
{
    bool canStart;
    [SerializeField] ArenaHandler arenaHandler;
    [SerializeField] GameObject musicObject, previousDoor; // our music, and the door we just came from

    private void Update()
    {
        if (canStart)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // disable all tooltips if they are on
                TooltipHandler.instance.SetTooltip(TooltipHandler.Tooltips.none);

                musicObject.SetActive(true);

                // try using our simple music manager as well
                try { SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.combat); } 
                catch { Debug.LogWarning("No SimpleMusicManager component found on directed music object, you may need to assign it in the inspector."); }

                // try to close the door
                try { previousDoor.SetActive(true); }
                catch { }


                arenaHandler.manualCombat = true;
                canStart = false;
                // set all of our children to inactive
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // safety teleport
        if (Input.GetKeyDown(KeyCode.F10))
        {
            PlayerController.instance.Teleport(transform.position + Vector3.up * 2);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canStart = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            canStart = false;
        }
    }
}
