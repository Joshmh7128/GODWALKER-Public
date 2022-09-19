using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool open = false, canOpen, triggerLock, triggerHit, distanceLock; // is this open? can we open it?
    [SerializeField] Animator animator;
    [SerializeField] float interactionDistance = 10f;
    [SerializeField] GameObject openMessage, lockParent;
    
    [SerializeField] List<ArenaHandler> associatedArenas = new List<ArenaHandler>();

    private void Start()
    {
        // link ourselves to all associated arenas
        LinkArenas();
    }

    // link each of our arenas to this door
    void LinkArenas()
    {
        // add ourselves to that arena's list of doors if we are not already in there
        foreach (var arena in associatedArenas)
        {
            if (!arena.doors.Contains(this))
            {
                arena.doors.Add(this);
            }
        }
    }

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
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance && !open && canOpen)
        {
            distanceLock = false;
            openMessage.SetActive(true);
        }
        else
        {
            distanceLock = true;
            openMessage.SetActive(false);
        }
    }

    // our door-opening input reads
    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpen == true && open == false && distanceLock == false)
        {
            Open();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if we have our trigger lock active, lock the door with the trigger
        if (triggerLock && other.transform.tag == "Player" && triggerHit == false)
        {
            TriggerLock();
        }
    }

    // to be run when the door animates and opens
    void Open()
    {
        open = true;
        openMessage.SetActive(false);
        animator.Play("DoorOpening");
        SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.intro);
    }

    // unlock to be used anywhere publicly
    public void Unlock()
    {
        canOpen = true;
        lockParent.SetActive(false);
    }

    // when our locking for specifically when the trigger is hit
    void TriggerLock()
    {
        // this happens when the player enters the room
        lockParent.SetActive(true);
        triggerHit = true;
    }

    // locking to be used anywhere publicly
    public void Lock()
    {
        lockParent.SetActive(true);
    }
}
