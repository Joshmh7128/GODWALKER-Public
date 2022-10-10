using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    public bool open = false, canOpen, triggerLock, triggerHit, triggerOverride, distanceLock, toCombat, keyLocked; // is this open? can we open it?
    [SerializeField] Animator animator;
    [SerializeField] float interactionDistance = 10f;
    [SerializeField] GameObject openMessage, lockParent, timerCanvasObject, keyMessage;

    [SerializeField] bool timed; // is this a timed door?
    [SerializeField] float timeRemaining = 60; // how much time is left?
    [SerializeField] Text timeText; 
    // each door should be associated with 2 rooms at maximum
    [SerializeField] List<ArenaHandler> associatedArenas = new List<ArenaHandler>();

    private void Start()
    {
        // link ourselves to all associated arenas
        LinkArenas();

        // if we're key locked, show it
        if (keyLocked) { keyMessage.SetActive(true); }
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
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance && !open && (canOpen || keyLocked))
        {
            distanceLock = false;
            if (keyLocked && PlayerStatManager.instance.keyAmount > 0)
                openMessage.SetActive(true);

            if (!keyLocked)
                openMessage.SetActive(true);
            
        }
        else
        {
            distanceLock = true;
            if (keyLocked && PlayerStatManager.instance.keyAmount > 0)
                openMessage.SetActive(false);

            if (!keyLocked)
                openMessage.SetActive(false);
        }
    }

    // our door-opening input reads
    void ProcessInput()
    {
        // if we can open
        if (Input.GetKeyDown(KeyCode.E) && canOpen == true && open == false && distanceLock == false)
        {
            Open();
        }

        // if we're attempting to open with a key
        if (Input.GetKeyDown(KeyCode.E) && keyLocked == true && open == false && distanceLock == false && PlayerStatManager.instance.keyAmount > 0)
        {
            PlayerStatManager.instance.keyAmount--;
            Open();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // if we have our trigger lock active, lock the door with the trigger
        if (triggerLock && other.transform.tag == "Player" && triggerHit == false && triggerOverride == false)
        {
            TriggerLock();
        }
    }

    // runs when combat begins
    public void CombatBegin()
    {
        if (timed)
        {
            timerCanvasObject.SetActive(true);
            StartCoroutine(Timer());
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (timeRemaining > 0)
        {
            timeRemaining -= 1f;
            timeText.text = timeRemaining.ToString(); // show how much time is left
            StartCoroutine(Timer()); // keep counting down
        }
        if (timeRemaining == 0)
        {
            timeText.text = "Locked. You didn't kill them fast enough.";
        }
    }

    // to be run when the door animates and opens
    void Open()
    {
        // open our door
        open = true;
        openMessage.SetActive(false);
        keyMessage.SetActive(false);
        animator.Play("DoorOpening");
        if (toCombat)
        {
            SimpleMusicManager.instance.PlaySong(SimpleMusicManager.MusicMoods.intro);
            // start combat in the correct arena
            foreach (ArenaHandler arena in associatedArenas)
            {
                if (!arena.combatBegun)
                {
                    arena.StartCombat();
                }
            }
        }
    }

    // unlock to be used anywhere publicly
    public void AttemptUnlock()
    {
        if (timed)
        { 
            if (timeRemaining > 0)
            {
                canOpen = true;
                lockParent.SetActive(false);
                timerCanvasObject.SetActive(false);
            }
        }
        else if (!keyLocked)
        {
            canOpen = true;
            lockParent.SetActive(false);
        } else if (keyLocked && PlayerStatManager.instance.keyAmount > 0)
        {
            PlayerStatManager.instance.keyAmount--;
            keyLocked = false;
            canOpen = true;
        }
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (ArenaHandler arena in associatedArenas)
        {
            Gizmos.DrawLine(transform.position, arena.transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(arena.transform.position, 3);
        }
    }
}
