using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualArenaStarter : MonoBehaviour
{
    bool canStart;
    [SerializeField] ArenaHandler arenaHandler;
    [SerializeField] GameObject musicObject;

    private void Update()
    {
        if (canStart)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                musicObject.SetActive(true);
                arenaHandler.manualCombat = true;
                Destroy(gameObject);    
            }
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
