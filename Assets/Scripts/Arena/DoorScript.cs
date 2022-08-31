using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool open = false, canOpen, triggerLock; // is this open? can we open it?
    [SerializeField] Animator animator;
    [SerializeField] float interactionDistance = 10f;
    [SerializeField] GameObject openMessage, lockParent;
    
    private void FixedUpdate()
    {
        ProcessDistance();
    }

    private void Update()
    {
        ProcessInput();
    }

    void ProcessDistance()
    {
        Transform player = PlayerController.instance.transform;
        // can we open?
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance && !open)
        {
            canOpen = true;
            openMessage.SetActive(true);
        }
        else
        {
            canOpen = false;
            openMessage.SetActive(false);
        }
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpen && !open)
        {
            openMessage.SetActive(false);
            open = true;
            animator.Play("DoorOpening");
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.intro);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if we have our trigger lock active, lock the door with the trigger
        if (triggerLock)
        {   // this happens when the player enters the room
            lockParent.SetActive(true);
        }
    }

    public void Unlock()
    {
        canOpen = true;
        lockParent.SetActive(false);
    }
}
